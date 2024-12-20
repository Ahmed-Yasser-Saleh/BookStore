using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO.Register
{
    public class RegisterDTO
    {
        public string fullname { get; set; }
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Username does not cantain any number")]
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string Confirmpassword { get; set; }
        [RegularExpression("[a-zA-Z0-9-_]+@gmail.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [Required]
        public string email { get; set; }

        [Length(11, 11)]
        [Phone]
        public string phonenumber { get; set; }
        public string address { get; set; }
        public bool isAdmin { get; set; }
    }
}
