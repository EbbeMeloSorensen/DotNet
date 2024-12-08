using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class ObservationRepository : Repository<Observation>, IObservationRepository
    {
        public ObservationRepository(DbContext context) : base(context)
        {
        }

        public override async Task Clear()
        {
            await Task.Run(() =>
            {
                var context = Context as ObsDBContext;

                context.RemoveRange(context.Observations);
            });
        }

        public override Task Update(Observation entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
