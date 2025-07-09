using Bookstore.DTO;
using Bookstore.DTO.Customer;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bookstore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Customers")]
    public class CustomerController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signmanager;

        public CustomerController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signmanager)
        {
            this.userManager = userManager;
            this.signmanager = signmanager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieves all customers", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "Returns a list of customers.", typeof(List<CustomerDTO>))]
        [SwaggerResponse(404, "No customers were found.")]
        public IActionResult Get()
        {
           // var Customers = db.customerrepository.Selectall();
           var Customers = userManager.GetUsersInRoleAsync("Customer").Result.OfType<Customer>().ToList();  
            List<CustomerDTO>  Customersdto = new List<CustomerDTO>();
            foreach (var customer in Customers)
            {
                var cs = new CustomerDTO();
                cs.fullname = customer.fullname;
                cs.username = customer.UserName;
                cs.email = customer.Email;
                cs.phonenumber = customer.PhoneNumber;
                cs.address = customer.address;
                Customersdto.Add(cs);
            }
            if (Customersdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "Customers Not Found" });
            return Ok(Customersdto);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer, Admin")]
        [SwaggerOperation(Summary = "Retrieves a customer by its ID.")]
        [SwaggerResponse(200, "Returns the customer details.", typeof(CustomerDTO))]
        [SwaggerResponse(404, "No customer was found with the given ID.")]
        public IActionResult GetbyId(string id)
        {
            var cs = (Customer)userManager.GetUsersInRoleAsync("Customer").Result.Where(c => c.Id == id).SingleOrDefault();
            if (cs == null) return NotFound(new { Status = 404, ErrorMassege = "Customer Not Found" });
            var CS = new CustomerDTO()
            {
                fullname = cs.fullname,
                username = cs.UserName,
                email = cs.Email,
                address = cs.address,
                phonenumber = cs.PhoneNumber
            };
            return Ok(CS);
        }
        [HttpPut]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "Updates an existing customer's details.")]
        [SwaggerResponse(200, "The customer details have been successfully updated.")]
        [SwaggerResponse(404, "No customer was found with the given ID.")]
        [SwaggerResponse(400, "The update failed due to validation errors or other issues.")]
        public IActionResult Edit(Edit CustomerEditDTO)
        {
            if (ModelState.IsValid)
            {
                var cs = (Customer)userManager.FindByIdAsync(CustomerEditDTO.Id).Result;
                if (cs == null) return NotFound(new { Status = 404, ErrorMassege = "Customer Not Found" });
                cs.Id = CustomerEditDTO.Id;
                cs.PhoneNumber = CustomerEditDTO.phonenumber;
                cs.address = CustomerEditDTO.address;
                cs.Email = CustomerEditDTO.email;
                cs.fullname = CustomerEditDTO.fullname;
                cs.UserName = CustomerEditDTO.username;
                var res = userManager.UpdateAsync(cs).Result;
                if (res.Succeeded)
                    return Ok();
                else
                    return BadRequest(new { Status = 400, ErrorMassege = "Edit Customer failed" });
                //db.customerrepository.Edit(cs);
                //db.Save();
                //return Ok();
            }
            else
                return BadRequest(ModelState);
        }

    }
}
    