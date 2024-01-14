using WIGOS.Domain.Entities.Geometry.Locations.Line;
using Craft.Persistence;

namespace WIGOS.Persistence.Repositories.Geometry
{
    public interface ILineRepository : IRepository<Line>
    {
        IList<Line> GetLinesIncludingPoints();
    }
}