using System;

namespace PR.Persistence
{
    public interface IUnitOfWorkFactoryHistorical : IUnitOfWorkFactory
    {
        DateTime? HistoricalTime { get; set; }
        bool IncludeHistoricalObjects { get; set; }
    }
}