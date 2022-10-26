using System.IO;
using System.Collections.Generic;
using System.Linq;
using Craft.IO.Utils;
using Newtonsoft.Json;
using DD.Domain;

namespace DD.IO
{
    public class DataIOHandler : IDataIOHandler
    {
        public void ExportDataToJson(
            IList<CreatureType> creatureTypes,
            string fileName)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(fileName));

            if (!directory.Exists)
            {
                directory.Create();
            }

            var ddData = new DDData
            {
                CreatureTypes = creatureTypes.ToList()
            };

            var jsonResolver = new ContractResolver();
            jsonResolver.IgnoreProperty(typeof(CreatureType), "Attacks");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = jsonResolver
            };

            var json = JsonConvert.SerializeObject(ddData, Formatting.Indented, settings);

            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.Write(json);
            }
        }
    }
}
