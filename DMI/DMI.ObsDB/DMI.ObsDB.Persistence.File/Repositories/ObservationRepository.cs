using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;
using DMI.ObsDB.Persistence.File;

namespace DMI.ObsDB.Persistence.File.Repositories
{
    public class ObservationRepository : IObservationRepository
    {
        public void Add(Observation entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
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

        public IEnumerable<Observation> Find(Expression<Func<Observation, bool>> predicate)
        {
            var result = predicate.Analyze();

            return null;
        }

        public IEnumerable<Observation> Find(IList<Expression<Func<Observation, bool>>> predicates)
        {
            var temp = predicates
                .Select(p => p.Analyze())
                .ToList();

            throw new NotImplementedException();
        }

        public IEnumerable<Observation> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Observation entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public Observation SingleOrDefault(Expression<Func<Observation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(Observation entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
