using C2IEDM.Domain.Entities.Geometry.Locations.Line;
using Craft.Persistence;

namespace C2IEDM.Persistence.Repositories.Geometry;

public interface ILineRepository : IRepository<Line>
{
    IList<Line> GetLinesIncludingPoints();
}