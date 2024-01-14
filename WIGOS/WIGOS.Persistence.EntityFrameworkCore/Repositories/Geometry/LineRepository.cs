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

        public override void Update(Line entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Line> entities)
        {
            throw new NotImplementedException();
        }

        public IList<Line> GetLinesIncludingPoints()
        {
            var lines = DbContext.Lines
                .Where(_ => _.Superseded == DateTime.MaxValue)
                .ToList();

            var lineObjectIds = lines
                .Select(_ => _.ObjectId)
                .ToList();

            var linePoints = DbContext.LinePoints
                .Where(_ => _.Superseded == DateTime.MaxValue && lineObjectIds.Contains(_.LineObjectId))
                .ToList();

            var pointObjectIds = linePoints
                .Select(_ => _.PointObjectId)
                .Distinct()
                .ToList();

            var points = DbContext.Points
                .Where(_ => _.Superseded == DateTime.MaxValue && pointObjectIds.Contains(_.ObjectId))
                .ToList();

            var pointMap = points.ToDictionary(_ => _.ObjectId, _ => _);

            // Hægt det sammen her

            throw new NotImplementedException();
        }
    }
}
