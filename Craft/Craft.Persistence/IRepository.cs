using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Craft.Persistence
{
    public interface IRepository<TEntity> where TEntity : class
    {
        int CountAll();
        int Count(Expression<Func<TEntity, bool>> predicate);
        int Count(IList<Expression<Func<TEntity, bool>>> predicates);

        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> Find(IList<Expression<Func<TEntity, bool>>> predicates);

        // This method was not in the videos, but I thought it would be useful to add.
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);
        Task UpdateRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        Task RemoveRange(IEnumerable<TEntity> entities);
        void Clear();

        // This is not part of the original pattern, but I made it in order to support bulk insert,
        // where we (rather than the database) dictate the ids
        void Load(IEnumerable<TEntity> entities);
    }
}
