using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class TimeSeriesRepository : Repository<TimeSeries>, ITimeSeriesRepository
    {
        public TimeSeriesRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            var context = Context as ObsDBContext;

            context.RemoveRange(context.TimeSeries);
            context.SaveChanges();
        }

        public override void Update(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }
    }
}
