using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Persistence.Repositories.WIGOS;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.WIGOS.AbstractEnvironmentalMonitoringFacilities
{
    public class AbstractEnvironmentalMonitoringFacilityRepository : Repository<AbstractEnvironmentalMonitoringFacility>, IAbstractEnvironmentalMonitoringFacilityRepository
    {
        public AbstractEnvironmentalMonitoringFacilityRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            var context = Context as WIGOSDbContextBase;

            context.RemoveRange(context.AbstractEnvironmentalMonitoringFacilities);
            context.SaveChanges();
        }

        public override void Update(AbstractEnvironmentalMonitoringFacility entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<AbstractEnvironmentalMonitoringFacility> entities)
        {
            throw new NotImplementedException();
        }
    }
}