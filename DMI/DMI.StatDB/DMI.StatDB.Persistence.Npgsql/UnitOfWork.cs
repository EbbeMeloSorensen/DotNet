using System;
using System.Transactions;
using DMI.StatDB.Persistence.Npgsql.Repositories;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.Npgsql
{
    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope _scope;

        public IStationRepository Stations { get; }
        public IPositionRepository Positions { get; }

        public UnitOfWork()
        {
            try
            {
                _scope = new TransactionScope();
                Stations = new StationRepository();
                Positions = new PositionRepository();
            }
            catch (Exception e)
            {
                _scope.Dispose();
                throw e;
            }
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public int Complete()
        {
            _scope.Complete();
            return 0;
        }
    }
}
