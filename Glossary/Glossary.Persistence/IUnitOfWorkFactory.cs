using System.Threading.Tasks;
using Craft.Logging;

namespace Glossary.Persistence
{
    public interface IUnitOfWorkFactory
    {
        string Host { get; set; }
        string Port { get; set; }
        string Database { get; set; }
        string Schema { get; set; }
        string User { get; set; }
        string Password { get; set; }

        void Initialize(ILogger logger);

        Task<bool> CheckRepositoryConnection();

        IUnitOfWork GenerateUnitOfWork();
    }
}
