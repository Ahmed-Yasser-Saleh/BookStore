using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO
{
    public class LoginDTO
    {
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Username does not cantain any number")]
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
