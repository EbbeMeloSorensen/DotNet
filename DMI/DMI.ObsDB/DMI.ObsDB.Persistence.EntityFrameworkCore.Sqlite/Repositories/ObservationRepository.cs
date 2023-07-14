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

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(Observation entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
