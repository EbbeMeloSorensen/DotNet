using System.Threading.Tasks;
using Craft.Logging;

namespace Glossary.Persistence
{
    public abstract class UnitOfWorkFactoryBase : IUnitOfWorkFactory
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public abstract void Initialize(ILogger logger);

        public abstract Task<bool> CheckRepositoryConnection();

        public abstract IUnitOfWork GenerateUnitOfWork();
    }
}
