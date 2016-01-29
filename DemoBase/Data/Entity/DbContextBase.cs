using System.Data.Entity;
using DemoBase.Table;

namespace DemoBase.Data.Entity
{
    public class DbContextBase: DbContext
    {
        protected DbContextBase(string connectionString)
            : base(connectionString)
        {
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }
    }

    public class DefaultDbContext : DbContextBase
    {
         public DefaultDbContext(string connectionString)
            : base(connectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CornerstoneLists>();
        }
    }
}
