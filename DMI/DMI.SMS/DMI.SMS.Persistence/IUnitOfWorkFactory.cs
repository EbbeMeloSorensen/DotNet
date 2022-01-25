using System.Threading.Tasks;
using Craft.Logging;

namespace DMI.SMS.Persistence
{
    public interface IUnitOfWorkFactory
    {
        void Initialize(ILogger logger);

        Task<bool> CheckRepositoryConnection();

        IUnitOfWork GenerateUnitOfWork();
    }
}