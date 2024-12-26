using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO
{
    public class LoginDTO
    {
        //[RegularExpression("[A-Za-z]+", ErrorMessage = "Username does not cantain any number")]
        //[Required]
        //public string username { get; set; }
        [RegularExpression("[a-zA-Z0-9-_]+@gmail.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
