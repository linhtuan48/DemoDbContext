using System;
using System.Data.Entity;

namespace DemoBase.Repository
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        private DbContext _ctx;
        private DbSet<T> _dbSet;

        public DbContext Context
        {
            get { return _ctx; }
            set { _ctx = value; }
        }

        public EntityRepository(DbContext context)
        {
            Context = context;
        }

        public DbSet<T> Table
        {
            get
            {
                if (_ctx == null)
                {
                    throw new NullReferenceException("Could not found instance of context");
                }

                return _ctx.Set<T>();
            }
        }

        public T Create(T entity)
        {
            if (_ctx == null)
            {
                throw new NullReferenceException("Could not found instance of context");
            }

            _ctx.Entry<T>(entity).State = EntityState.Added;
            Commit();
            return entity;
        }

        public int Commit()
        {
            return _ctx.SaveChanges();
        }
    }
}