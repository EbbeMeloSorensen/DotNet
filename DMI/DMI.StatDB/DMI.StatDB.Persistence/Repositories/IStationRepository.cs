using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Persistence;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence.Repositories
{
    public interface IStationRepository : IRepository<Station>
    {
        Station GetStation(int statid);

        Station GetStationWithPositions(int statid);

        IEnumerable<Station> GetAllStationsWithPositions();

        IEnumerable<Station> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate);

        IEnumerable<Station> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates);
    }
}
