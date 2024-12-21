using Bookstore.Contex;
using Bookstore.Model;

namespace Bookstore.Repository
{
    public class UnitOfwork
    {
        BookstoreContext db;
        GenericRepository<OrderDetails> OrderDetailsrepository;
        GenericRepository<Customer> Customerrepository;
        CatalogRepository Catalogrepository;
        AuthorRepository AuthorRepository;
        OrderRepository OrderRepository;
        BookRepository BookRepository;
        public UnitOfwork(BookstoreContext db)
        {
            this.db = db;
        }
        public BookRepository bookrepository
        {
            get
            {
                if (BookRepository == null)
                {
                    BookRepository = new BookRepository(db);
                }
                return BookRepository;
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
        public OrderRepository Orderepository
        {
            get
            {
                if (OrderRepository == null)
                {
                    OrderRepository = new OrderRepository(db);
                }
                return OrderRepository;
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
