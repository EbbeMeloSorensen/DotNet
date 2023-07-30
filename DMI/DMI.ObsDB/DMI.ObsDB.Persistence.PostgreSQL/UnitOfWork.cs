using System;
using System.Transactions;
using DMI.ObsDB.Persistence.Repositories;
using DMI.ObsDB.Persistence.PostgreSQL.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        //private TransactionScope _scope;

        public IObservingFacilityRepository ObservingFacilities { get; }

        public ITimeSeriesRepository TimeSeries { get; }

        public IObservationRepository Observations { get; }

        public UnitOfWork()
        {
            try
            {
                // Dette har jeg været nødt til at udkommentere, da det giver andledning til flere problemer:
                // 1: Man kan slet ikke læse fra nanoq-ro (nanoq4), fordi den er en "host standby", hvilket
                //    ikke er kompatibelt med "serializable mode"...
                // 2: Man kan godt læse fra nanoq3, men man kan så ikke skrive til en sqlite database, da
                //    den så brokker sig over noget med "ambient transactions"... 
                // NB: Konstruktionen er mindet på at sikre transaktionskontrol i forbindelse med skrivning
                //     til databasen, så det burde ikke være noget problem, når man bare skal læse som her
                //    
                //_scope = new TransactionScope();

                ObservingFacilities = new ObservingFacilityRepository();
                TimeSeries = new TimeSeriesRepository();
                Observations = new ObservationRepository();
            }
            catch (Exception e)
            {
                //_scope.Dispose();
                throw e;
            }
        }

        public int Complete()
        {
            //_scope.Complete();
            return 0;
        }

        public void Dispose()
        {
            //_scope.Dispose();
        }
    }
}
