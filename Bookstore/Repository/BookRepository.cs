using Bookstore.Contex;
using Bookstore.Model;

namespace Bookstore.Repository
{
    public class BookRepository : GenericRepository<Book>
    {
        public BookRepository(BookstoreContext db) : base(db)
        {
            
        }

        public List<Book> Search(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var Books = db.Books.Where(ctg => ctg.Title.Contains(name)).ToList();
                return Books;
            }
            return db.Books.ToList();
        }
    }
}
