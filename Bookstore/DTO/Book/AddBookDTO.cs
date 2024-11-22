namespace Bookstore.DTO
{
    public class AddBookDTO
    {
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int CatalogId { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateOnly PublishDate { get; set; }
    }
}
