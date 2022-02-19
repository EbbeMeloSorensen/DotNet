using System;
using System.Collections.Generic;
using System.IO;
using DMI.StatDB.Domain.Entities;
using Newtonsoft.Json;
using Craft.IO.Utils;

namespace DMI.StatDB.IO
{
    public class DataIOHandler : IDataIOHandler
    {
        public void ExportDataToXML(
            IList<Station> stationInformations, 
            string fileName)
        {
            throw new NotImplementedException();
        }

        public void ExportDataToJson(
            IList<Station> stations, 
            string fileName)
        {
            var json = JsonConvert.SerializeObject(
                stations, 
                Formatting.Indented, 
                new DoubleJsonConverter(), 
                new NullableDoubleJsonConverter());

            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(json);
            }
        }

        public void ImportDataFromXML(
            string fileName, 
            out IList<Station> stationInformations)
        {
            throw new NotImplementedException();
        }

        public void ImportDataFromJson(
            string fileName, 
            out IList<Station> stations)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                stations = JsonConvert.DeserializeObject<List<Station>>(json);
            }
        }
    }
}