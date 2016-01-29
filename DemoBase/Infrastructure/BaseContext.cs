using System.Data.Entity;

namespace DemoBase.Infrastructure
{
    public class BaseContext<TContext> : DbContext where TContext : DbContext
    {
        static BaseContext()
        {
            Database.SetInitializer<TContext>(null);
        }

        protected BaseContext()
            : base("name=RemoveBackgroundEntities")
        { }
    }
}