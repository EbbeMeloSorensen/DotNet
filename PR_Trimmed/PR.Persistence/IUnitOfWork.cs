using System;
using PR.Persistence.Repositories;

namespace PR.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ISmurfRepository Smurfs { get; }

        IPersonRepository People { get; }
        IPersonCommentRepository PersonComments { get; }

        void Clear();

        void Complete();
    }
}
