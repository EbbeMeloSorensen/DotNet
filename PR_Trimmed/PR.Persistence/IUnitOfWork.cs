using System;
using PR.Persistence.Repositories;

namespace PR.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonRepository People { get; }
        IPersonAssociationRepository PersonAssociations { get; }

        int Complete();
    }
}
