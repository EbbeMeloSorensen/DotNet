using System;
using System.Transactions;
using DMI.ObsDB.Persistence.Repositories;
using DMI.ObsDB.Persistence.PostgreSQL.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope _scope;

        public IObservingFacilityRepository ObservingFacilities { get; }

        public ITimeSeriesRepository TimeSeries { get; }

        public IObservationRepository Observations { get; }

        public UnitOfWork()
        {
            try
            {
                _scope = new TransactionScope();

                ObservingFacilities = new ObservingFacilityRepository();
                TimeSeries = new TimeSeriesRepository();
                Observations = new ObservationRepository();
            }
            catch (Exception e)
            {
                _scope.Dispose();
                throw e;
            }
        }

        public int Complete()
        {
            _scope.Complete();
            return 0;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
