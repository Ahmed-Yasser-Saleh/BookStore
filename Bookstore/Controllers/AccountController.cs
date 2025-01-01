
using Bookstore.DTO;
using Bookstore.DTO.Account;
using Bookstore.DTO.Password;
using Bookstore.DTO.Register;
using Bookstore.Model;
using Bookstore.Repository;
using Bookstore.TokenManagerService;
using Microsoft.AspNetCore.Authorization;

//using Castle.Core.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                return BadRequest(new { Status = 400, ErrorMassege = "Password must match ConfirmPassword" }); 
            }

            if (Rg.isAdmin)
            {
                var user = await userManager.FindByNameAsync(Rg.username);
                if (user != null)
                {
                    return BadRequest(new { Status = 400, ErrorMassege = "Register failed, There is Admin with the same username" });
                }
                var user2 = await userManager.FindByEmailAsync(Rg.email);
                if (user2 != null)
                {
                    return BadRequest(new { Status = 400, ErrorMassege = "Register failed, There is Admin with the same email" });
                }
                var Adm = new Admin
                {
                    Email = Rg.email,
                    UserName = Rg.username,
                    PhoneNumber = Rg.phonenumber,
                };

                var res = await userManager.CreateAsync(Adm, Rg.password);
                if (res.Succeeded)
                {
                    //var admin = await userManager.FindByEmailAsync(Rg.email);
                    //var confirmgeneratedtoken = await userManager.GenerateEmailConfirmationTokenAsync(admin);
                    //if (rs.Succeeded)
                    //{
                    //    return Ok(new { Status = "Admin registered successfully.",
                    //                    confirmedtoken =  $"{confirmgeneratedtoken}" });
                    //}
                    //else
                    //{
                    //    return BadRequest("AddToRole failed: " + string.Join(", ", rs.Errors.Select(e => e.Description)));
                    //}
                    var rs = await userManager.AddToRoleAsync(Adm, "Admin");
                    if (rs.Succeeded)
                    {
                        return Ok(new { Status = "Admin registered successfully." });
                    }
                    else
                    {
                        return BadRequest(new { Status = 400, ErrorMassege = "AddToRole failed: " + string.Join(", ", rs.Errors.Select(e => e.Description)) });
                    }
                }

                else
                {
                    return BadRequest(new { Status = 400, ErrorMassege = "Create failed: " + string.Join(", ", res.Errors.Select(e => e.Description)) });
                }
            }
            else
            {
                var user = await userManager.FindByNameAsync(Rg.username);
                if (user != null)
                {
                    return BadRequest(new { Status = 400, ErrorMassege = "Register failed, There is customer with the same username" });
                }
                var user2 = await userManager.FindByEmailAsync(Rg.email);
                if (user2 != null)
                {
                    return BadRequest(new { Status = 400, ErrorMassege = "Register failed, There is customer with the same email" });
                }
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
                    //var confirmgeneratedtoken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var rs = await userManager.AddToRoleAsync(cust, "Customer");
                    //if (rs.Succeeded)
                    //{
                    //    return Ok(new { Status = $"Customer registered successfully. please confirm your email with code: {confirmgeneratedtoken}" });
                    //}
                    var rs = await userManager.AddToRoleAsync(cust, "Customer");
                    if (rs.Succeeded)
                    {
                        return Ok(new { Status = "Customer registered successfully." });
                    }
                    else
                    {
                        return BadRequest(new { Status = 400, ErrorMassege = "AddToRole failed: " + string.Join(", ", rs.Errors.Select(e => e.Description)) });
                    }
                }
                else
                {
                    return BadRequest(new { Status = 400, ErrorMassege = "Create failed: " + string.Join(", ", res.Errors.Select(e => e.Description)) });
                }
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO cs)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(cs.email);
                if (user == null)
                    return Unauthorized(new { Status = 401, ErrorMassege = "Unauthorized" });
                //var isconfirmed = await userManager.IsEmailConfirmedAsync(user);
                //if (!isconfirmed)
                //    return BadRequest("Email Not Confirmed");
                var rs = signmanager.PasswordSignInAsync(user.UserName, cs.password, false, false).Result;
                if (rs.Succeeded)
                {
                    #region generate token

                    List<Claim> userdata = new List<Claim>();
                    userdata.Add(new Claim(ClaimTypes.Email, user.Email));
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
                    return Ok(new { token = tokenstring });
                    #endregion
                }
                else
                    return BadRequest(new { Status = 400, ErrorMassege = "Login Failed" });
            }
            else
                return Ok(); //BadRequest(ModelState);
        }
        [HttpPost("ChangePassword")]
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult ChangePassword(ChangePasswordDTO passwordDTO)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmailAsync(passwordDTO.email).Result;
                if (user == null)
                {
                    return BadRequest(new { Status = 401, ErrorMassege = $"user not exist with email: {passwordDTO.email}" });
                }
                if (passwordDTO.newpassword != passwordDTO.confirmpassword)
                {
                    return BadRequest(new { Status = 401, ErrorMassege = "confirm password does not match new password" });
                }
                var result = userManager.ChangePasswordAsync(user, passwordDTO.oldpassword, passwordDTO.newpassword).Result;
                if (result.Succeeded)
                {
                    return Ok(new { Status = "Password changed" });
                }
                else
                    return BadRequest(new { Status = 401, ErrorMassege = "old password is not correct" });
            }
            else
                return BadRequest(ModelState);
        }

        //[HttpPost("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(string email, string Confirmedtoken)
        //{
            
        //    if (string.IsNullOrEmpty(Confirmedtoken))
        //    {
        //        return BadRequest("Invalid token.");
        //    }

        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return BadRequest($"There is no customer with email: {email}");
        //    }

        //    var isconfirmed = await userManager.ConfirmEmailAsync(user, Confirmedtoken);
        //    if (isconfirmed.Succeeded)
        //    {
        //        return Ok("Email confirmed successfully!");
        //    }
        //    else
        //    {
        //        return BadRequest("Email confirmation failed");
        //    }
            
        //}

        [HttpGet("Logout")]
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult Logout()
        {
            // Extract the token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
            // Add the token to the blacklist
            TokenManager.AddToBlacklist(token);
        
            // Sign out the user
            signmanager.SignOutAsync();
        
            return Ok(new { Status = "Logged out successfully." });
        }
    }
}
