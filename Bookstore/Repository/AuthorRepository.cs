using Bookstore.Contex;
using Bookstore.Model;

namespace Bookstore.Repository
{
    public class AuthorRepository : GenericRepository<Author>
    {
        public AuthorRepository(BookstoreContext db):base(db)
        {
            
        }
        public bool CheckId(int id)
        {
            var x = db.Authors.Any(c => c.Id == id);
            return x;
        }
    }
}
