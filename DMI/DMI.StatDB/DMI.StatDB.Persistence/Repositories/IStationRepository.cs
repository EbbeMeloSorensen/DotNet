using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Persistence;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence.Repositories
{
    public interface IStationRepository : IRepository<Station>
    {
        Station Get(int statid);

        Station GetWithPositions(int statid);

        IEnumerable<Station> GetAllStationsWithPositions();

        Task<IEnumerable<Station>> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate);

        Task<IEnumerable<Station>> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates);
    }
}
