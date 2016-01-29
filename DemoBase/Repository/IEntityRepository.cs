using System.Linq;
using System.Linq.Expressions;

namespace DemoBase.Repository
{
    public interface IEntityRepository<T> where T : class
    {
        T Create(T entity);
    }
}