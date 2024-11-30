using Bookstore.Model;

namespace Bookstore.DTO.Order
{
    public class GetorderDTO
    {
        public string CustomerId { get; set; }
        public List<OrderdetailsDTO> orderDetails { get; set; }
        public string Status { get; set; }
    }
}
