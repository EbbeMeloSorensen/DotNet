using System;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonRepository People { get; }
        IPersonAssociationRepository PersonAssociations { get; }

        int Complete();
    }
}
