using System.Collections.Generic;
using Craft.Persistence;
using PR.Domain.Entities.C2IEDM.Geometry.Locations;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Line;

namespace PR.Persistence.Repositories.C2IEDM.Geometry
{
    public interface ILocationRepository : IRepository<Location>
    {
    }

    public interface ILineRepository : IRepository<Line>
    {
        IList<Line> GetLinesIncludingPoints();
    }    

    public interface ILinePointRepository : IRepository<LinePoint>
    {
    }    
}
