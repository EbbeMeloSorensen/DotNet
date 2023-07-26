using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class TimeSeriesRepository : ITimeSeriesRepository
    {
        public void Add(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<TimeSeries, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeSeries> Find(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeSeries> Find(IList<Expression<Func<TimeSeries, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public TimeSeries Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeSeries> GetAll()
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(int id)
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(int id, DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(int id, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public TimeSeries SingleOrDefault(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }
    }
}
