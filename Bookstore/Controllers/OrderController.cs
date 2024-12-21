using Bookstore.DTO.Order;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Orders")]
    public class OrderController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        UnitOfwork db;

        public OrderController(UnitOfwork db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new order for a customer.")]
        [SwaggerResponse(201, "The order was created successfully.")]
        [SwaggerResponse(400, "The input is invalid or a book is out of stock.")]
        [SwaggerResponse(404, "The customer does not exist.")]
        public async Task<IActionResult> Add(AddorderDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return BadRequest("Empty input");
            }
            var user = await userManager.FindByIdAsync(orderDTO.CustomerId);
            if (user == null)
                return NotFound("Customer not exist");
            var neworder = new Order
            {
                CustomerId = user.Id,
                OrderDate = DateTime.Now,
                Status = Order_Status.CREATED
            };
            //db.orderrepository.Add(neworder);
            //db.Save();
            decimal totalprice = 0;
            foreach (var order in orderDTO.orderDetails)
            {
                var book = db.bookrepository.GetById(order.BookId);
                if (book == null)
                {
                    return BadRequest($"book with id: {order.BookId} not exist");
                }
                if (neworder.orderDetails.Any(x => x.BookId == book.Id))
                    return BadRequest("book id has choossed");

                if (book.Stock < order.quantity)
                {
                    return BadRequest($"No enugh books for {book.Title} in stock");
                }
                var orderdetails = new OrderDetails
                {
                    OrderId = neworder.Id,
                    BookId = order.BookId,
                    Quantity = order.quantity,
                    UnitPrice = book.Price
                };
                neworder.orderDetails.Add(orderdetails); //save it in table direct instead of repo and need to db.save()
                book.Stock -= order.quantity;
                totalprice += order.quantity * book.Price;
                db.bookrepository.Edit(book);
                db.orderDetailsrepository.Add(orderdetails);
            }
            neworder.TotalPrice = totalprice;
            db.Orderepository.Edit(neworder);
            db.Save();
            return Created();
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves order details by order ID.")]
        [SwaggerResponse(200, "Returns the order details.", typeof(GetorderDTO))]
        [SwaggerResponse(404, "The order was not found.")]
        public async Task<IActionResult> Get(int id)
        {
            var order = db.Orderepository.GetById(id);
            if (order == null) return NotFound("order not found");
            var stsvar = true;
            if (order.Status == Order_Status.CANCELED)
                stsvar = false;
            var cs = (Customer)userManager.FindByIdAsync(order.CustomerId).Result;
            var ord = new GetorderDTO {
                CustomerName = cs.fullname,
                orderDetails = order.orderDetails.Select(x => new OrderdetailsDTO
                {
                    BookId = x.BookId,
                    quantity = x.Quantity,
                    Unitprice = x.UnitPrice
                }).ToList(),
                Status = stsvar ? "CREATED" : "CANCELED"
            };
            decimal Totalprice = 0;
            foreach (var item in ord.orderDetails)
            {
                Totalprice += item.Unitprice * item.quantity;
            }
            ord.totalprice = Totalprice;
            return Ok(ord);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Updates the status of an order.", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "The order status was updated successfully.")]
        [SwaggerResponse(404, "The order was not found.")]
        public async Task<IActionResult> updatestatus(int id, string status)
        {
            var order = db.Orderepository.GetById(id);
            if (order == null) return NotFound("order not found");

            if(status == "CANCELED")
                order.Status = Order_Status.CANCELED;
            else if(status == "CREATED")
                order.Status = Order_Status.CREATED;
            else
            {
                return BadRequest("Please, Enter Valid Status");
            }
            db.Orderepository.Edit(order);
            db.Save();
            return Ok();
        }
        [HttpGet("Customer/{id}")]
        [SwaggerOperation(Summary = "Get all orders for customer.")]
        [SwaggerResponse(200, "return all orders for customer")]
        [SwaggerResponse(404, "The order with customerid was not found.")]
        public IActionResult GetforCustomer(string id)
        {
            var orders = db.Orderepository.GetByCustomerID(id);
            if (orders == null) return NotFound();
            var cs = (Customer)userManager.FindByIdAsync(id).Result;
            var ordersDTO = new List<GetorderDTO>();
            foreach (var order in orders)
            {
                var ord = new GetorderDTO
                {
                    CustomerName = cs.fullname,
                    orderDetails = order.orderDetails.Select(x => new OrderdetailsDTO
                    {
                        BookId = x.BookId,
                        quantity = x.Quantity,
                        Unitprice = x.UnitPrice
                    }).ToList(),
                    Status = order.Status.ToString()
                };
                decimal Totalprice = 0;
                foreach (var item in ord.orderDetails)
                {
                    Totalprice += item.Unitprice * item.quantity;
                }
                ord.totalprice = Totalprice;
                ordersDTO.Add(ord);
            }
            if(ordersDTO.Count == 0) return NotFound($"There are no orders for Customer: {cs.fullname}");
            return Ok(ordersDTO);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete an order.", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "The order status was deleted successfully.")]
        [SwaggerResponse(404, "The order was not found.")]
        public IActionResult Cancel(int id)
        {
            var order = db.Orderepository.GetById(id);
            if (order == null) return NotFound("order not found");
            db.Orderepository.Delete(order);
            db.Save();
            return Ok();
        }

    }
}
