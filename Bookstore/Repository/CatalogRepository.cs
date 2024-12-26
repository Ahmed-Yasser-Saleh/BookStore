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
        public bool Checkname(string name)
        {
            var status = db.Catalogs.Any(ctg => ctg.Name.ToUpper() == name.ToUpper());
            return status;
        }
        public List<Catalog> Search(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var catalogs = db.Catalogs.Where(ctg => ctg.Name.Contains(name)).ToList();
                return catalogs;
            }
            return db.Catalogs.ToList();
        }
    }
}
