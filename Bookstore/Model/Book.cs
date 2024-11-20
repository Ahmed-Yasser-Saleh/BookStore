using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bookstore.Model
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [ForeignKey("author")]
        public int AuthorId { get; set; }
        [ForeignKey("catalog")]
        public int CatalogId { get; set; }
        [Column (TypeName = "money")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateOnly PublishDate { get; set; }
        public virtual Author author { get; set; }
        public virtual Catalog catalog { get; set; }
        public virtual List<OrderDetails> orderDetails { get; set; } = new List<OrderDetails>();
    }
}
