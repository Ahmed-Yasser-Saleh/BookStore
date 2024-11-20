using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bookstore.DTO
{
    public class CustomerDTO
    {
        public string fullname { get; set; }
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Username does not cantain any number")]
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [RegularExpression("[a-zA-Z0-9-_]+@gmail.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [Required]
        public string email { get; set; }
        [MaxLength(11)]
        public string phonenumber { get; set; }
        public string address { get; set; }
    }
}
