using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.Npgsql.Repositories
{
    internal class PositionRepository : IPositionRepository
    {
        public Position Get(decimal id)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Position, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Position> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Position> Find(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Position> Find(IList<Expression<Func<Position, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Position SingleOrDefault(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(Position entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Position entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Position entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }
    }
}
