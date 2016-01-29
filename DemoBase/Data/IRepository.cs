using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DemoBase.Data
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> Table { get; }

        IQueryable<TTable> GetTable<TTable>() where TTable : class;

        int Count();

        void Delete(T entity, bool deleteRelated = false);

        void Delete(Expression<Func<T, bool>> filterExpression);

        void DeleteMany(IEnumerable<T> entities);

        T GetById(object id);

        void Insert(T entity, params Expression<Func<T, dynamic>>[] includePaths);

        void InsertMany(IEnumerable<T> entities);

        void Update(T entity, params Expression<Func<T, dynamic>>[] includePaths);

        void UpdateMany(IEnumerable<T> entities);

        void UpdateMany(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression);
    }
}
