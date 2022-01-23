using System.Threading.Tasks;
using Craft.Logging;

namespace DD.Persistence
{
    public interface IUnitOfWorkFactory
    {
        void Initialize(ILogger logger);

        Task<bool> CheckRepositoryConnection();

        IUnitOfWork GenerateUnitOfWork();
    }
}
