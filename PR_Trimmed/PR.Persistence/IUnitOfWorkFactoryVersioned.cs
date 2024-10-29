using System;

namespace PR.Persistence
{
    public interface IUnitOfWorkFactoryVersioned : IUnitOfWorkFactory
    {
        DateTime? DatabaseTime { get; set; }
    }

    public interface IUnitOfWorkFactoryHistorical : IUnitOfWorkFactory
    {
        DateTime? HistoricalTime { get; set; }
    }
}
