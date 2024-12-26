using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO.Account
{
    public class ChangePasswordDTO
    {
        [RegularExpression("[a-zA-Z0-9-_]+@gmail.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [Required]
        public string email {  get; set; }
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
        public string confirmpassword { get; set; }
    }
}
