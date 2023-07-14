using System.Linq;
using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class ObservingFacilityRepository : Repository<ObservingFacility>, IObservingFacilityRepository
    {
        public ObservingFacilityRepository(DbContext context) : base(context)
        {
        }

        public ObservingFacility Get(
            int id)
        {
            var context = Context as ObsDBContext;

            return context.ObservingFacilities
                .SingleOrDefault(_ => _.Id == id) ?? throw new InvalidOperationException();
        }

        public ObservingFacility GetObservingFacilityIncludingTimeSeries(
            int id)
        {
            var context = Context as ObsDBContext;

            return context.ObservingFacilities
                .Include(_ => _.TimeSeries)
                .SingleOrDefault(_ => _.Id == id) ?? throw new InvalidOperationException();
        }

        public override void Clear()
        {
            var context = Context as ObsDBContext;

            context.RemoveRange(context.ObservingFacilities);
            context.SaveChanges();
        }

        public override void Update(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }
    }
}
