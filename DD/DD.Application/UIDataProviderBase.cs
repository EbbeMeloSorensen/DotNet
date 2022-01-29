using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using DD.Domain;
using DD.IO;

namespace DD.Application
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

        public void Initialize(
            ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task<bool> CheckConnection();

        public abstract void CreateCreatureType(
            CreatureType creatureType);

        public abstract CreatureType GetCreatureType(
            int id);

        public abstract IList<CreatureType> GetAllCreatureTypes();

        public event EventHandler<CreatureTypeEventArgs> CreatureTypeCreated;

        protected virtual void OnCreatureTypeCreated(CreatureType creatureType)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = CreatureTypeCreated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new CreatureTypeEventArgs(creatureType));
            }
        }
    }
}
