using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using Craft.Persistence.EntityFramework;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.EntityFramework.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        public StationRepository(
            DbContext context) : base(context)
        {
        }

        public override void Update(
            Station station)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<Station> stations)
        {
            throw new NotImplementedException();
        }

        public Station GetStationWithPositions(
            int statid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> GetAllStationsWithPositions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            throw new NotImplementedException();
        }
    }
}
