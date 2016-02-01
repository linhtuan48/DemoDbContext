using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DemoBase.Factory
{
    public class DaoFactory : IDaoFactory
    {
        private DbContext _ctx;
        private readonly Dictionary<Type, DbContext> _ctxCollection;
        private bool _disposed;
        private readonly object _obj = new object();

        public DaoFactory()
        {
            _ctxCollection = new Dictionary<Type, DbContext>();
        }

        public T GetContext<T>() where T : DbContext
        {
            if (_ctx == null || typeof(T) != _ctx.GetType())
            {
                lock (_obj)
                {
                    Type type = typeof(T);
                    if (_ctxCollection.ContainsKey(type))
                    {
                        _ctx = _ctxCollection[type];
                    }
                    else
                    {
                        _ctx = Activator.CreateInstance<T>();
                        _ctxCollection.Add(type, _ctx);
                    }
                }
            }

            return _ctx as T;
        }

        public void Dispose()
        {
            lock (_obj)
            {
                if (_ctxCollection != null && _ctxCollection.Count > 0)
                {
                    foreach (var item in _ctxCollection)
                    {
                        if (item.Value != null && item.Value.Database.Connection.State != System.Data.ConnectionState.Closed)
                        {
                            item.Value.Database.Connection.Close();
                            item.Value.Dispose();
                        }
                    }
                }

                if (_ctxCollection != null) _ctxCollection.Clear();
            }

            GC.SuppressFinalize(this);
        }
    }
}