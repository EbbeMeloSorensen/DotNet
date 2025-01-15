using System.Collections.Generic;
using Craft.Persistence;

namespace PR.Persistence.Repositories.C2IEDM.Geometry.Locations.Line
{
    public interface ILineRepository : IRepository<Domain.Entities.C2IEDM.Geometry.Locations.Line.Line>
    {
        IList<Domain.Entities.C2IEDM.Geometry.Locations.Line.Line> GetLinesIncludingPoints();
    }
}
