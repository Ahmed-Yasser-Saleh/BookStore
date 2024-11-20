using Bookstore.DTO;
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
        UnitOfwork db;

        public CustomerController(UnitOfwork db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signmanager)
        {
            this.db = db;
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
        [HttpPost("Login")]
        public IActionResult Login(LoginCustomerDTO cs) {
            if (ModelState.IsValid)
            {
                var rs = signmanager.PasswordSignInAsync(cs.username, cs.password, false, false).Result;
                if (rs.Succeeded)
                {
                    #region generate token
                    string key = "My Complex Secret Key";
                    var x = new List<Claim> {
                    new Claim(ClaimTypes.Name, cs.username)
                    };
                    var sercretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var signture = new SigningCredentials(sercretkey, SecurityAlgorithms.Aes128CbcHmacSha256);
                    var token = new JwtSecurityToken(
                        claims: x,
                        signingCredentials: signture
                        );
                    var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
                    #endregion
                    return Ok(tokenstring);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest(ModelState);
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
            var Customers = db.customerrepository.Selectall();
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
    }
}
    