using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Model
{
    public class Order
    {
        public int Id { get; set; }
        [ForeignKey("customer")]
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public virtual Customer customer { get; set; }
        public virtual List<OrderDetails> orderDetails { get; set; } = new List<OrderDetails>();
    }
}
