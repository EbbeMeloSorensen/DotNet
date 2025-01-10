using System;
using PR.Persistence.Repositories;

namespace PR.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonRepository People { get; }
        IPersonCommentRepository PersonComments { get; }

        void Clear();

        void Complete();
    }
}
