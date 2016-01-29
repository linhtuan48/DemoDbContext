using System.Data.Entity;
using DemoBase.Table;

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



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CornerstoneLists>();
        }
    }
}
