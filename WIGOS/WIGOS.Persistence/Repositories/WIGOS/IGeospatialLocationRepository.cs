using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using Craft.Persistence;

namespace WIGOS.Persistence.Repositories.WIGOS
{
    public interface IGeospatialLocationRepository : IRepository<GeospatialLocation>
    {
        GeospatialLocation Get(
            Guid id);
    }
}