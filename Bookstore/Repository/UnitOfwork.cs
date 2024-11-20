using Bookstore.Contex;
using Bookstore.Model;

namespace Bookstore.Repository
{
    public class UnitOfwork
    {
        BookstoreContext db;
        GenericRepository<Book> Bookrepository;
        GenericRepository<Author> Authorrepository;
        GenericRepository<Order> Orderrepository;
        CatalogRepository Catalogrepository;
        public UnitOfwork(BookstoreContext db)
        {
            this.db = db;
        }
        public GenericRepository<Book> bookrepository { get {
                if(Bookrepository == null)
                {
                    Bookrepository = new GenericRepository<Book>(db);
                }
                return Bookrepository;
            } }
        public GenericRepository<Author> authorrepository
        {
            get
            {
                if (Authorrepository == null)
                {
                    Authorrepository = new GenericRepository<Author>(db);
                }
                return Authorrepository;
            }
        }
        public GenericRepository<Order> orderrepository
        {
            get
            {
                if (Orderrepository == null)
                {
                    Orderrepository = new GenericRepository<Order>(db);
                }
                return Orderrepository;
            }
        }
        public CatalogRepository catalogrepository
        {
            get
            {
                if (Catalogrepository == null)
                {
                    Catalogrepository = new CatalogRepository(db);
                }
                return Catalogrepository;
            }
        }
        public void Save()
        {
            db.SaveChanges();
        }

    }
}
