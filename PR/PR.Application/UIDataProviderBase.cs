using System;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;
using PR.IO;

namespace PR.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
        protected ILogger _logger;
        private readonly IDataIOHandler _dataIOHandler;

        protected UIDataProviderBase(
            IDataIOHandler dataIOHandler)
        {
            _dataIOHandler = dataIOHandler;
        }

        public virtual void Initialize(
            ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task<bool> CheckConnection();

        public abstract void CreatePerson(Person person);

        public abstract Person GetPerson(
            Guid id);
    }
}
