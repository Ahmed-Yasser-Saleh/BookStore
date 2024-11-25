using Bookstore.DTO;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Admins")]
    public class AdminController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signmanager;

        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signmanager)
        {
            this.userManager = userManager;
            this.signmanager = signmanager;
        }
        [HttpPost("Register")]
        [SwaggerOperation(Summary = "Register Admin", Tags = new[] { "Admin Operations" })]
        public async Task<IActionResult> Register(AdminDTO cs) //username: ahmedyasser pw: 12345
        {
            if (ModelState.IsValid)
            {
                var Adm = new Admin
                {
                    Email = cs.email,
                    UserName = cs.username,
                    PhoneNumber = cs.phonenumber
                };
                var res = await userManager.CreateAsync(Adm, cs.password);
                if (res.Succeeded)
                {
                    var rs = await userManager.AddToRoleAsync(Adm, "Admin");
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
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
