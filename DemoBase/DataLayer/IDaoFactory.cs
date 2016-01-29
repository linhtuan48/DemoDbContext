using System.Data.Entity;
using DemoBase.Data.Entity;

namespace DemoBase.DataLayer
{
    public interface IDaoFactory
    {
        DefaultDbContext GetContext();

    }

    public class DaoFactory : IDaoFactory
    {
        private DefaultDbContext _context;

        private bool _disposed;

        public DaoFactory()
        {
            Database.SetInitializer<DefaultDbContext>(null);
        }
        public DefaultDbContext GetContext()
        {
            return _context = new DefaultDbContext("DefaultConnection");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                        _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
