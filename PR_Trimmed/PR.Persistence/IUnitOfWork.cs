using System;
using PR.Persistence.Repositories;

namespace PR.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonRepository People { get; }

        void Complete();
    }
}
