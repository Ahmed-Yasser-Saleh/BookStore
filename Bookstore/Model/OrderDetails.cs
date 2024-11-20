using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Bookstore.Model
{
    public class OrderDetails
    {
        [ForeignKey("order")]
        public int OrderId { get; set; }
        [ForeignKey("book")]
        public int BookId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        public virtual Book book { get; set; }
        public virtual Order order { get; set; }
    }
}
