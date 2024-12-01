using Bookstore.Model;

namespace Bookstore.DTO.Order
{
    public class GetorderDTO
    {
        public string CustomerName { get; set; }
        public List<OrderdetailsDTO> orderDetails { get; set; }
        public decimal totalprice { get; set; }
        public string Status { get; set; }
    }
}
