using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        private StatDbContext StatDbContext
        {
            get { return Context as StatDbContext; }
        }

        public StationRepository(
            StatDbContext context) : base(context)
        {
        }

        public Station Get(int statid)
        {
            return StatDbContext.Stations.Single(s => s.StatID == statid);
        }

        public Station GetWithPositions(
            int statid)
        {
            return StatDbContext.Stations
                .Include(s => s.Positions)
                .SingleOrDefault(s => s.StatID == statid) ?? throw new InvalidOperationException();
        }

        public IEnumerable<Station> GetAllStationsWithPositions()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Station>> FindStationsWithPositions(Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Station>> FindStationsWithPositions(IList<Expression<Func<Station, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public override Task Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(Station entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }
    }
}
