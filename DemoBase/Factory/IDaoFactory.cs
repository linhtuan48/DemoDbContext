using System;
using System.Data.Entity;

namespace DemoBase.Factory
{
    public interface IDaoFactory : IDisposable
    {
        T GetContext<T>() where T : DbContext;
    }
}