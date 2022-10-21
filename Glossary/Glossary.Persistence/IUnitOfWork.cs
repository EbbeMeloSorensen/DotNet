using System;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IRecordRepository Records { get; }
        IRecordAssociationRepository RecordAssociations { get; }

        int Complete();
    }
}
