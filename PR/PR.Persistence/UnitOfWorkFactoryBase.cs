using System.Threading.Tasks;

namespace PR.Persistence
{
    public abstract class UnitOfWorkFactoryBase : IUnitOfWorkFactory
    {
        public abstract IUnitOfWork GenerateUnitOfWork();
    }
}
