using DemoBase.Table;
using System.Data.Entity;

namespace DemoBase.Infrastructure
{
    public class DemoDbContext : BaseContext<DemoDbContext>
    {
        public DemoDbContext()
            : base("name=RemoveBackgroundEntities")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<CornerstoneLists> CornerstoneLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CornerstoneLists>();
        }
    }
}