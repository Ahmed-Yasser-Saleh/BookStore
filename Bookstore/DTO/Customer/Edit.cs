using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO.Customer
{
    public class Edit
    {
        [Required]
        public String Id { get; set; }
        public string fullname { get; set; }
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Username does not cantain any number")]
        [Required]
        public string username { get; set; }
        [RegularExpression("[a-zA-Z0-9-_]+@gmail.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [Required]
        public string email { get; set; }

        [Length(11, 11)]
        public string phonenumber { get; set; }
        public string address { get; set; }
    }
}
