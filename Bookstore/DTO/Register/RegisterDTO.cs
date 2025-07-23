using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTO.Register
{
    public class RegisterDTO
    {
        [RegularExpression("[A-Za-z]+", ErrorMessage = "name can not cantain any number")]
        [Required]
        public string fullname { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string Confirmpassword { get; set; }
        [RegularExpression("[a-zA-Z0-9-_]+@gmail\\.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [Required]
        public string email { get; set; }

        [RegularExpression("01[0125][0-9]{8}", ErrorMessage = "Phone number must be a valid Egyptian number.")]
                public string phonenumber { get; set; }    
        public string address { get; set; }
        public bool isAdmin { get; set; }
    }
}
