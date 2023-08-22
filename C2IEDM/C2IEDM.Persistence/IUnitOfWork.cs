using C2IEDM.Persistence.Repositories.Geometry;

namespace C2IEDM.Persistence;

public interface IUnitOfWork : IDisposable
{
    ILocationRepository Locations { get; }
    IPointRepository Points { get; }
    IAbsolutePointRepository AbsolutePoints { get; }

    int Complete();
}