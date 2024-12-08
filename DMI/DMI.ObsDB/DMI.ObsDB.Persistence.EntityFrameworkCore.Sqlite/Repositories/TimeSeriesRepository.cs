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

        public TimeSeries Get(
            int id)
        {
            var context = Context as ObsDBContext;

            return context.TimeSeries
                .SingleOrDefault(_ => _.Id == id) ?? throw new InvalidOperationException();
        }

        public TimeSeries GetIncludingObservations(
            int id)
        {
            var context = Context as ObsDBContext;

            return context.TimeSeries
                .Include(t => t.Observations)
                .SingleOrDefault(t => t.Id == id) ?? throw new InvalidOperationException();
        }

        public TimeSeries GetIncludingObservations(
            int id,
            DateTime startTime)
        {
            var context = Context as ObsDBContext;

            return context.TimeSeries
                .Include(t => t.Observations
                    .Where(o => o.Time >= startTime))
                .SingleOrDefault(t => t.Id == id) ?? throw new InvalidOperationException();
        }

        public TimeSeries GetIncludingObservations(
            int id,
            DateTime startTime,
            DateTime endTime)
        {
            var context = Context as ObsDBContext;

            return context.TimeSeries
                .Include(t => t.Observations
                    .Where(o => o.Time >= startTime && o.Time <= endTime))
                .SingleOrDefault(t => t.Id == id) ?? throw new InvalidOperationException();
        }

        public override async Task Clear()
        {
            await Task.Run(() =>
            {
                var context = Context as ObsDBContext;

                context.RemoveRange(context.TimeSeries);
            });
        }

        public override Task Update(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }
    }
}
