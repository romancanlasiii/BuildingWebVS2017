using Microsoft.EntityFrameworkCore;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.EF
{
    public class StoreContext : DbContext
    {
        //private string _connectionString;

        public StoreContext ()
        {

        }

        public StoreContext (DbContextOptions options) : base (options)
        {
            
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SpyStore;Trusted_Connection=True;MultipleActiveResultSets=true;");
                    //options => options.ExecutionStrategy (c => new CustomExecutionStrategy(c)));
            }
        }

        public virtual DbSet<Category> Categories { get; set; }
    }
}
