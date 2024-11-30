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
        GenericRepository<OrderDetails> OrderDetailsrepository;
        GenericRepository<Customer> Customerrepository;
        CatalogRepository Catalogrepository;
        AuthorRepository AuthorRepository;
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
        public GenericRepository<Customer> customerrepository
        {
            get
            {
                if (Customerrepository == null)
                {
                    Customerrepository = new GenericRepository<Customer>(db);
                }
                return Customerrepository;
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
        public GenericRepository<OrderDetails> orderDetailsrepository
        {
            get
            {
                if (OrderDetailsrepository == null)
                {
                    OrderDetailsrepository = new GenericRepository<OrderDetails>(db);
                }
                return OrderDetailsrepository;
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
        public AuthorRepository authorRepository
        {
            get
            {
                if (AuthorRepository == null)
                {
                    AuthorRepository = new AuthorRepository(db);
                }
                return AuthorRepository;
            }
        }
        public void Save()
        {
            db.SaveChanges();
        }

    }
}
