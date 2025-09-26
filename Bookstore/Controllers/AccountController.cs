
using Bookstore.DTO;
using Bookstore.DTO.Account;
using Bookstore.DTO.Email;
using Bookstore.DTO.Password;
using Bookstore.DTO.Register;
using Bookstore.Email;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authorization;

//using Castle.Core.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using ResetPasswordDTO = Bookstore.DTO.Account.ResetPasswordDTO;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    //[ApiController] //to restrict app to show defaultvalidation of modelstate
   [ApiExplorerSettings(GroupName = "Account")]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signmanager;
        EmailService emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signmanager, EmailService emailService)
        {
            this.userManager = userManager;
            this.signmanager = signmanager;
            this.emailService = emailService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO Rg)
        {
            if (Rg == null || !ModelState.IsValid)
            {
                var errors = ModelState.Values
              .SelectMany(v => v.Errors)
              .Select(e => e.ErrorMessage)
              .ToList();

                return BadRequest(new { Status = 400, ErrorMassege = errors });
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
                    //send active email
                    string code = await userManager.GenerateEmailConfirmationTokenAsync(Adm);
                    await emailService.SendEmail(Adm.Email, code, "confirm", "ActiveEmail", "Please Active your Email");
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
                    //send active email
                    string code = await userManager.GenerateEmailConfirmationTokenAsync(cust);
                    await emailService.SendEmail(cust.Email, code, "confirm", "ActiveEmail", "Please Active your Email");
                  /*  if(cust.EmailConfirmed = false)
                        return BadRequest(new {Status = 400, Errormea})*/
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
        public async Task<IActionResult> Login([FromBody] LoginDTO cs)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(cs.email);
                if (user == null)
                    return Unauthorized(new { Status = 401, ErrorMassege = "Unauthorized" });
                if(user.EmailConfirmed == false)
                    return BadRequest(new {Status = 404, ErrorMassege = "Email Not Confirmed" });
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
                    return BadRequest(new { Status = 404, ErrorMassege = "Login Failed" });
            }
            else
                return Ok(); //BadRequest(ModelState);
        }
        [HttpPost("ChangePassword")]
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO passwordDTO)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmailAsync(passwordDTO.email).Result;
                if (user == null)
                {
                    return BadRequest(new { Status = 404, ErrorMassege = $"user not exist with email: {passwordDTO.email}" });
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
        [HttpGet("Logout")]
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult Logout()
        { 
            // Sign out the user
            signmanager.SignOutAsync();
        
            return Ok(new { Status = "Logged out successfully." });
        }
        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest(new { Status = 404, ErrorMassege = "Invalid user." });
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new { Status = 404, ErrorMassege = $"Invalid token.{token}" });
            }

            return Ok("Your email has been confirmed successfully!");
        }

        [HttpPost("send-email-ForgotPassword")]
        public async Task<IActionResult> SendEmailForgotpw(string email)
        {
            var user = userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                return BadRequest(new { Status = 404, ErrorMassege = $"user not exist with email: {email}" });
            }
            if(user.EmailConfirmed == false)
            {
                return BadRequest(new { Status = 404, ErrorMassege = "Email Not Confirmed, please confirm Email First then try again"});
            }
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            await emailService.SendEmailResetPassword(user.Email, code, "ResetPassword", "ForgotPassword", "Please click on button to reset Password");
            return Ok("Email Sent Successfuly");
        }

        [HttpPost("ResetPassword")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO, string email, string token)
        {
            if (resetPasswordDTO.newpassword != resetPasswordDTO.confirmpassword)
            {
                return BadRequest(new { Status = 400, ErrorMessage = "Password must match Confirm Password" });
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { Status = 404, ErrorMessage = "Invalid user." });
            }

            var result = await userManager.ResetPasswordAsync(user, token, resetPasswordDTO.newpassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { Status = 400, ErrorMessage = "Invalid or expired token", Errors = result.Errors });
            }

            return Ok("Password reset successfully");
        }

        [HttpDelete("Delete/{email}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> DeleteAcc(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
                return NotFound(new { Status = 400, ErrorMessage = $"No User with id: {email}" });
            var result = userManager.DeleteAsync(user).Result;
            if(!result.Succeeded)
                return BadRequest(new { Status = 400, ErrorMessage = $"{result.Errors}" });
            return Ok();
        }
    }
}
