using System.Transactions;
using DD.Persistence.Repositories;
using DD.Persistence.Npgsql.Repositories;

namespace DD.Persistence.Npgsql
{
    public class UnitOfWork : IUnitOfWork
    {
        // Jeg kan ikke helt huske hvorfor jeg tog dette med..
        private TransactionScope _scope;

        public ICreatureTypeRepository CreatureTypes { get; }
        public ISceneRepository Scenes { get; }

        public UnitOfWork()
        {
            _scope = new TransactionScope();
            CreatureTypes = new CreatureTypeRepository();
            Scenes = new SceneRepository();
        }

        public int Complete()
        {
            _scope.Complete();
            return 0;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
