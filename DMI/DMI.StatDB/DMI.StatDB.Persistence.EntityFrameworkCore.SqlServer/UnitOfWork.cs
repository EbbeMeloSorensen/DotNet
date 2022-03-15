using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        public IStationRepository Stations => throw new NotImplementedException();

        public IPositionRepository Positions => throw new NotImplementedException();

        public int Complete()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
