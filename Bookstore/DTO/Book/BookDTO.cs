using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string CatalogName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? image { get; set; }
        public DateOnly PublishDate { get; set; }
    }
}
