using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DMI.StatDB.Domain.Entities;
using Newtonsoft.Json;
using Craft.IO.Utils;

namespace DMI.StatDB.IO
{
    public class DataIOHandler : IDataIOHandler
    {
        public void ExportDataToXML(
            IList<Station> stationInformations,
            IList<Position> positions,
            string fileName)
        {
            throw new NotImplementedException();
        }

        public void ExportDataToJson(
            IList<Station> stations, 
            IList<Position> positions, 
            string fileName)
        {
            var jsonResolver = new ContractResolver();
            jsonResolver.IgnoreProperty(typeof(Station), "Positions");
            jsonResolver.IgnoreProperty(typeof(Position), "Id");
            jsonResolver.IgnoreProperty(typeof(Position), "Station");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = jsonResolver
            };

            var statDBData = new StatDBData
            {
                Stations = stations.ToList(),
                Positions = positions.ToList()
            };

            var json = JsonConvert.SerializeObject(
                statDBData, 
                Formatting.Indented, 
                settings);

            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(json);
            }
        }

        public void ImportDataFromXML(
            string fileName, 
            out IList<Station> stationInformations,
            out IList<Position> positions)
        {
            throw new NotImplementedException();
        }

        public void ImportDataFromJson(
            string fileName, 
            out IList<Station> stations,
            out IList<Position> positions)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                var statDBData = JsonConvert.DeserializeObject<StatDBData>(json);
                stations = statDBData.Stations;
                positions = statDBData.Positions;
            }

            var nextId = 1;

            // We don't read ids from the json file, so we have to generate them
            positions
                .ToList()
                .ForEach(p => p.Id = nextId++);
        }
    }
}