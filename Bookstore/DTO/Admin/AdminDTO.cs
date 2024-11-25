using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Bookstore.DTO
{
    public class AdminDTO
    {
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Username does not cantain any number")]
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        //   [RegularExpression("[a-zA-Z0-9-_]+@gmail.com", ErrorMessage = "Please enter a valid Gmail address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid Gmail address")]
        [Required]
        public string email { get; set; }

        [Phone]
        [Length(11,11)]
        public string phonenumber { get; set; }
    }
}
