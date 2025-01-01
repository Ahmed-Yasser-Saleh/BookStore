using Microsoft.AspNetCore.Identity;

namespace Bookstore.Model
{
    public class Admin : IdentityUser
    {
        public string? Image { get; set; }
    }
}
