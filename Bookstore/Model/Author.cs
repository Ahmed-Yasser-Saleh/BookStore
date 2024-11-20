namespace Bookstore.Model
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public int Age { get; set; }
        public virtual List<Book> Books { get; set; } = new List<Book>();
    }
}
