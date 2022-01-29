using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using DD.Domain;

namespace DD.Application
{
    public interface IUIDataProvider
    {
        void Initialize(
            ILogger logger);

        Task<bool> CheckConnection();

        void CreateCreatureType(
            CreatureType creatureType);

        CreatureType GetCreatureType(
            int id);

        IList<CreatureType> GetAllCreatureTypes();

        event EventHandler<CreatureTypeEventArgs> CreatureTypeCreated;
    }
}
