using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence.Repositories.WIGOS;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.WIGOS.AbstractEnvironmentalMonitoringFacilities;

public class ObservingFacilityRepository : Repository<ObservingFacility>, IObservingFacilityRepository
{
    public ObservingFacilityRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(ObservingFacility entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        var updatedObservingFacilities= observingFacilities.ToList();
        var ids = updatedObservingFacilities.Select(p => p.Id);
        var observingFacilitiesFromRepository = Find(p => ids.Contains(p.Id)).ToList();

        observingFacilitiesFromRepository.ForEach(ofRepo =>
        {
            var updatedObservingFacility = updatedObservingFacilities.Single(ofUpd => ofUpd.Id == ofRepo.Id);

            ofRepo.Name = updatedObservingFacility.Name;
            ofRepo.DateEstablished = updatedObservingFacility.DateEstablished;
            ofRepo.DateClosed = updatedObservingFacility.DateClosed;
        });
    }
}
