﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Npgsql;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.Npgsql.Repositories
{
    public class StationKeeperRepository : IStationKeeperRepository
    {
        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<StationKeeper, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<StationKeeper, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public StationKeeper Get(
            decimal id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StationKeeper>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StationKeeper>> Find(
            Expression<Func<StationKeeper, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StationKeeper>> Find(
            IList<Expression<Func<StationKeeper, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public StationKeeper SingleOrDefault(
            Expression<Func<StationKeeper, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Add(
            StationKeeper entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(
            IEnumerable<StationKeeper> entities)
        {
            throw new NotImplementedException();
        }

        public Task Update(
            StationKeeper entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(
            IEnumerable<StationKeeper> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(
            StationKeeper entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(
            IEnumerable<StationKeeper> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<StationKeeper> entities)
        {
            throw new NotImplementedException();
        }
    }
}
