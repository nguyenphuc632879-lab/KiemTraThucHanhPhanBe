using System.Data.Entity;

namespace MyStore_MVC5.Models
{
    public class DBEntities : DbContext
    {
        public DBEntities() : base("name=DBEntities")
        {
        }

        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
