using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO.Account
{
    public class ResetPasswordDTO
    {
        public string newpassword { get; set; }
        public string confirmpassword { get; set; }
    }
}
