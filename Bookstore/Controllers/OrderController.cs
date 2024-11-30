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
                Status = "Created",
            };
            db.orderrepository.Add(neworder);
            db.Save();
            decimal totalprice = 0;
            foreach (var order in orderDTO.orderDetails)
            {
                var book = db.bookrepository.GetById(order.BookId);
                if (book == null)
                    return BadRequest($"book with id: {order.BookId} not exist");
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
               book.Stock -= order.quantity;
               totalprice += order.quantity * book.Price;
               db.bookrepository.Edit(book);
               db.orderDetailsrepository.Add(orderdetails);
            }
            neworder.TotalPrice = totalprice;
            db.orderrepository.Edit(neworder);
            db.Save();
            return Created();
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves order details by order ID.")]
        [SwaggerResponse(200, "Returns the order details.", typeof(GetorderDTO))]
        [SwaggerResponse(404, "The order was not found.")]
        public async Task<IActionResult> Get(int id)
        {
            var order = db.orderrepository.GetById(id);
            if (order == null) return NotFound("order not found");
            var ord = new GetorderDTO {
                CustomerId = order.CustomerId,
                orderDetails = order.orderDetails.Select(x => new OrderdetailsDTO
                {
                    BookId = x.BookId,
                    quantity = x.Quantity
                }).ToList(),
                Status = order.Status
            };
            return Ok(ord);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Updates the status of an order.", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "The order status was updated successfully.")]
        [SwaggerResponse(404, "The order was not found.")]
        public async Task<IActionResult> updatestatus(int id, string status)
        {
            var order = db.orderrepository.GetById(id);
            if (order == null) return NotFound("order not found");
            order.Status = status;
            db.orderrepository.Edit(order);
            db.Save();
            return Ok();
        }

    }
}
