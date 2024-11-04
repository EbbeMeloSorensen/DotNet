using System;

namespace PR.Persistence.APIClient
{
    public class UnitOfWorkFactory : IUnitOfWorkFactoryVersioned, IUnitOfWorkFactoryHistorical
    {
        public DateTime? HistoricalTime { get; set; }
        public bool IncludeHistoricalObjects { get; set; }
        public DateTime? DatabaseTime { get; set; }

        public void Initialize(
            bool versioned)
        {
            // Here we would normally have made sure the database existed and possibly seeded it,
            // but we don't do anything, when the unit of work represents an API

            // We might obtain the token here, though..
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(
                HistoricalTime,
                IncludeHistoricalObjects,
                DatabaseTime);
        }

        public void Reseed()
        {
            throw new System.NotImplementedException();
        }
    }
}
