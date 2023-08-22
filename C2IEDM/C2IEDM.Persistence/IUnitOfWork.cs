using C2IEDM.Persistence.Repositories.Geometry;

namespace C2IEDM.Persistence;

public interface IUnitOfWork : IDisposable
{
    ILocationRepository LocationRepository { get; }
    IPointRepository PointRepository { get; }
    IAbsolutePointRepository AbsolutePointRepository { get; }

    int Complete();
}