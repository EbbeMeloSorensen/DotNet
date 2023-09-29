using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using C2IEDM.Persistence.Repositories.WIGOS;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.WIGOS.GeospatialLocations;

public class PointRepository : Repository<Point>, IPointRepository
{
    public PointRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(Point entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(
        IEnumerable<Point> points)
    {
        throw new NotImplementedException();
    }
}
