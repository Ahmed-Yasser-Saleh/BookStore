using Bookstore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace Bookstore.Contex
{
    public class BookstoreContext : IdentityDbContext
    {
        public BookstoreContext()
        {
            
        }
        public BookstoreContext(DbContextOptions<BookstoreContext> op) :base(op)
        {

        }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Catalog> Catalogs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OrderDetails>().HasKey(od => new { od.OrderId, od.BookId });
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole("Admin"),
                new IdentityRole("Customer")
                );
        }
    }
}
