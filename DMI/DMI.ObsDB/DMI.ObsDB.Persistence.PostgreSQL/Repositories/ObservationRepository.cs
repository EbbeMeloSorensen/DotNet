using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class ObservationRepository : IObservationRepository
    {
        public Task Add(Observation entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Observation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Observation, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Observation>> Find(Expression<Func<Observation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Observation>> Find(IList<Expression<Func<Observation, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Observation>> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Observation entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public Observation SingleOrDefault(Expression<Func<Observation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Update(Observation entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
