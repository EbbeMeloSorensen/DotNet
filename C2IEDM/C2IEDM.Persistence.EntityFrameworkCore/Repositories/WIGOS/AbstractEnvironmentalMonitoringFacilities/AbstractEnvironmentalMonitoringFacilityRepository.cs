using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence.Repositories.WIGOS;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.WIGOS.AbstractEnvironmentalMonitoringFacilities;

public class AbstractEnvironmentalMonitoringFacilityRepository : Repository<AbstractEnvironmentalMonitoringFacility>, IAbstractEnvironmentalMonitoringFacilityRepository
{
    public AbstractEnvironmentalMonitoringFacilityRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        var context = Context as C2IEDMDbContextBase;

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
