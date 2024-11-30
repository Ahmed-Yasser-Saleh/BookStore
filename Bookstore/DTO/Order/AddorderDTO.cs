namespace Bookstore.DTO.Order
{
    public class AddorderDTO
    {
        public string CustomerId { get; set; }
        public List<OrderdetailsDTO> orderDetails { get; set; }
    }
}
