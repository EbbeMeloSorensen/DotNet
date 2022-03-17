using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        public StationRepository(
            StatDbContext context) : base(context)
        {
        }

        public Station GetStation(int statid)
        {
            var statDbContext = Context as StatDbContext;

            return statDbContext.Stations.Single(s => s.StatID == statid);
        }

        public Station GetStationWithPositions(int statid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> GetAllStationsWithPositions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> FindStationsWithPositions(Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> FindStationsWithPositions(IList<Expression<Func<Station, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public override void Update(Station entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }
    }
}
