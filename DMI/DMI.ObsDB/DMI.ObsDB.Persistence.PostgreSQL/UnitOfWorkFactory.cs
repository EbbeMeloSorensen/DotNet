using System;

namespace DMI.ObsDB.Persistence.PostgreSQL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork();
        }
    }
}
