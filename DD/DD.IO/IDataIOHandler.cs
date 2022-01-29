using System.Collections.Generic;
using DD.Domain;

namespace DD.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToJson(
            IList<CreatureType> creatureTypes,
            string fileName);
    }
}
