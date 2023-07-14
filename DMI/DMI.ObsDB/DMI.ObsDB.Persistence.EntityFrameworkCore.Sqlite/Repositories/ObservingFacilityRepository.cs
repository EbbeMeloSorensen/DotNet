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
