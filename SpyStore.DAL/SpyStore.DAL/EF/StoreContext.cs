using Microsoft.EntityFrameworkCore;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.EF
{
    public class StoreContext : DbContext
    {
        //private string _connectionString;

        #region Constructor
        public StoreContext ()
        {

        }

        public StoreContext (DbContextOptions options) : base (options)
        {
            
        }
        #endregion

        #region Methods
        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SpyStore;Trusted_Connection=True;MultipleActiveResultSets=true;");
                    //options => options.ExecutionStrategy (c => new CustomExecutionStrategy(c)));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e =>
                e.EmailAddress).HasName("IX_Customers").IsUnique();
            });

        }
        #endregion

        #region DB Set Properties
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }
        #endregion
    }
}
