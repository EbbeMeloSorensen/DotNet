using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class LineRepository : Repository<Line>, ILineRepository
    {
        private WIGOSDbContextBase DbContext => Context as WIGOSDbContextBase;

        public LineRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(Line entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Line> entities)
        {
            throw new NotImplementedException();
        }

        public IList<Line> GetLinesIncludingPoints()
        {
            // Retrieve all the LATEST version of the line objects that haven't been superceded or deleted
            var lines = DbContext.Lines
                .Where(_ => _.Superseded == DateTime.MaxValue)
                .ToList();

            // Identify the object ids of those objects (we need those ids for identifying the relevant line points)
            var lineObjectIds = lines
                .Select(_ => _.ObjectId)
                .ToList();

            // Retrieve all the LATEST version of relevant line points
            var linePoints = DbContext.LinePoints
                .Where(_ => _.Superseded == DateTime.MaxValue && lineObjectIds.Contains(_.LineObjectId))
                .ToList();

            // As before, identify the ids of the line point objects in order to identify the relevant points
            var pointObjectIds = linePoints
                .Select(_ => _.PointObjectId)
                .Distinct()
                .ToList();

            // Retrieve all the LATEST version of relevant points
            var points = DbContext.Points
                .Where(_ => _.Superseded == DateTime.MaxValue && pointObjectIds.Contains(_.ObjectId))
                .ToList();

            // Notice that this function only facilitates retrieval of the LATEST version - so no retrospection here...

            var pointMap = points.ToDictionary(_ => _.ObjectId, _ => _);

            // Hægt det sammen her

            throw new NotImplementedException();
        }
    }
}
