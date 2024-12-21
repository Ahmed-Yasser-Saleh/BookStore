using Bookstore.Contex;
using Bookstore.DTO.Author;
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

        public List<Author> Search(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var authors = db.Authors.Where(ath => ath.FullName.Contains(name)).ToList();
                return authors;
            }
            return db.Authors.ToList();
        }

    }
}
