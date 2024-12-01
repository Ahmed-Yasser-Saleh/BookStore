using Bookstore.Contex;
using Bookstore.Model;

namespace Bookstore.Repository
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(BookstoreContext db):base(db)
        {
            
        }
        public List<Order> GetByCustomerID(string Customerid)
        {
            var OrderRecords = db.Orders
              .Where(a => a.CustomerId == Customerid)
              .ToList();
            return OrderRecords;
        }
    }
}
