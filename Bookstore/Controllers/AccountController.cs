using Bookstore.DTO;
using Bookstore.DTO.Account;
using Bookstore.DTO.Register;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [ApiExplorerSettings(GroupName = "Account")]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signmanager;
        //UnitOfwork db;

        public AccountController(UnitOfwork db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signmanager)
        {
          //  this.db = db;
            this.userManager = userManager;
            this.signmanager = signmanager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO Rg)
        {
            if (Rg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Rg.password != Rg.Confirmpassword)
            {
                return BadRequest("Password must match ConfirmPassword");
            }

            if (Rg.isAdmin)
            {
                var Adm = new Admin
                {
                    Email = Rg.email,
                    UserName = Rg.username,
                    PhoneNumber = Rg.phonenumber,
                };

                var res = await userManager.CreateAsync(Adm, Rg.password);
                if (res.Succeeded)
                {
                    var rs = await userManager.AddToRoleAsync(Adm, "Admin");
                    if (rs.Succeeded)
                    {
                        return Ok("Admin registered successfully.");
                    }
                    else
                    {
                        return BadRequest("AddToRole failed: " + string.Join(", ", rs.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    return BadRequest("Create failed: " + string.Join(", ", res.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                var cust = new Customer
                {
                    fullname = Rg.fullname,
                    Email = Rg.email,
                    address = Rg.address,
                    UserName = Rg.username,
                    PhoneNumber = Rg.phonenumber
                };

                var res = await userManager.CreateAsync(cust, Rg.password);
                if (res.Succeeded)
                {
                    var rs = await userManager.AddToRoleAsync(cust, "Customer");
                    if (rs.Succeeded)
                    {
                        return Ok("Customer registered successfully.");
                    }
                    else
                    {
                        return BadRequest("AddToRole failed: " + string.Join(", ", rs.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    return BadRequest("Create failed: " + string.Join(", ", res.Errors.Select(e => e.Description)));
                }
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO cs)
        {
            if (ModelState.IsValid)
            {
                var rs = signmanager.PasswordSignInAsync(cs.username, cs.password, false, false).Result;
                if (rs.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(cs.username);
                    #region generate token

                    List<Claim> userdata = new List<Claim>();
                    userdata.Add(new Claim(ClaimTypes.Name, user.UserName));
                    userdata.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Any())
                    {
                        userdata.Add(new Claim(ClaimTypes.Role, roles.First()));
                    }
                    string key = "My Complex Secret Key My Complex Secret Key My Complex Secret Key";
                    var secertkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

                    var signingcer = new SigningCredentials(secertkey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                    claims: userdata,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signingcer
                    );
                    var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(tokenstring);
                    #endregion
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordDTO passwordDTO)
        {
            var user = userManager.FindByNameAsync(passwordDTO.username).Result;
            if(user == null) {
                return BadRequest("user not exist");
            }
            if(passwordDTO.newpassword != passwordDTO.confirmpassword) {
                return BadRequest("Please, Enter the confirm password with your new password to confirm");
            }
            var result = userManager.ChangePasswordAsync(user, passwordDTO.oldpassword, passwordDTO.newpassword).Result;
            if (result.Succeeded)
            {
                return Ok("Password changed");
            }
            else
               return BadRequest("old password is not correct");
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            signmanager.SignOutAsync();
            //token will be not valid 
            return Ok();
        }
    }
}
