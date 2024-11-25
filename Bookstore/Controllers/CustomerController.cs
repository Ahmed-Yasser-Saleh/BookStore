using Bookstore.DTO;
using Bookstore.DTO.Customer;
using Bookstore.Model;
using Bookstore.Repository;
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
    public class CustomerController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signmanager;
       // UnitOfwork db;

        public CustomerController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signmanager)
        {
            //this.db = db;
            this.userManager = userManager;
            this.signmanager = signmanager;
        }
        [HttpPost("Register")]
        public IActionResult Register(CustomerDTO cs) //username: ahmedyasser pw: 12345
        {
            if (ModelState.IsValid)
            {
                var cust = new Customer
                {
                    fullname = cs.fullname,
                    Email = cs.email,
                    address = cs.address,
                    UserName = cs.username,
                    PhoneNumber = cs.phonenumber
                };
                //var res = userManager.CreateAsync(cust).Result; //function direct hash the password and save it
                var res = userManager.CreateAsync(cust, cs.password).Result;
                if (res.Succeeded)
                {
                    var rs = userManager.AddToRoleAsync(cust, "Customer").Result;
                    if (rs.Succeeded)
                    {
                        return Ok();
                    }
                    else
                        return BadRequest("addtorole not succeded");
                }
                else
                {
                    return BadRequest("create not succeded");
                }
            } else
            {
                return BadRequest(ModelState);
            }
        }
      
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            signmanager.SignOutAsync();
            //token will be not valid 
            return Ok();
        }
        [HttpGet]
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
                return NotFound();
            return Ok(Customersdto);
        }
        [HttpGet("{id}")]
        public IActionResult GetbyId(string id)
        {
            var cs = (Customer)userManager.GetUsersInRoleAsync("Customer").Result.Where(c => c.Id == id).SingleOrDefault();
            if (cs == null) return NotFound();
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
        public IActionResult Edit(Edit CustomerEditDTO)
        {
            if (ModelState.IsValid)
            {
                var cs = (Customer)userManager.FindByIdAsync(CustomerEditDTO.Id).Result;
                if (cs == null) return NotFound();
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
                    return BadRequest();
                //db.customerrepository.Edit(cs);
                //db.Save();
                //return Ok();
            }
            else
                return BadRequest(ModelState);
        }
    }
}
    