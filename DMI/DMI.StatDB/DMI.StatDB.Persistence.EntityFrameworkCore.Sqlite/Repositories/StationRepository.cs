using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Craft.Persistence;
using Craft.Persistence.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        private StatDBContext DBContext
        {
            get { return Context as StatDBContext; }
        }

        public StationRepository(DbContext context) : base(context)
        {
        }

        public override Task Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(
            Station entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(
            IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }

        public Station Get(
            int statid)
        {
            return DBContext.Stations.Single(s => s.StatID == statid);
        }

        public Station GetWithPositions(
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
            var stations = predicates.Any()
                ? DBContext.Stations
                    .Include(_ => _.Positions)
                    .Where(predicates.Aggregate((c, n) => c.And(n)))
                    .ToList()
                : DBContext.Stations
                    .Include(_ => _.Positions)
                    .ToList();

            return stations;
        }
    }
}
