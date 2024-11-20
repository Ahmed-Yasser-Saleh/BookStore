using Bookstore.Contex;
using Bookstore.Model;

namespace Bookstore.Repository
{
    public class CatalogRepository : GenericRepository<Catalog>
    {
        public CatalogRepository(BookstoreContext db) : base(db)
        {

        }
        public bool CheckId(int id)
        {
            var x = db.Catalogs.Any(c => c.Id == id);
            return x;
        }
    }
}
