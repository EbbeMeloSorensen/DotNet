using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Craft.IO.Utils;
using Glossary.Domain.Entities;
using Record = Glossary.Domain.Entities.Record;

namespace Glossary.IO
{
    public class DataIOHandler : IDataIOHandler
    {
        private XmlSerializer _xmlSerializer;

        private XmlSerializer XmlSerializer
        {
            get
            {
                if (_xmlSerializer != null)
                    return _xmlSerializer;

                var xOver = new XmlAttributeOverrides();
                var attrs = new XmlAttributes { XmlIgnore = true };
                xOver.Add(typeof(Record), "ObjectRecords", attrs);
                xOver.Add(typeof(Record), "SubjectRecords", attrs);
                xOver.Add(typeof(RecordAssociation), "SubjectRecord", attrs);
                xOver.Add(typeof(RecordAssociation), "ObjectRecord", attrs);
                _xmlSerializer = new XmlSerializer(typeof(GlossaryData), xOver);

                return _xmlSerializer;
            }
        }

        public void ExportDataToXML(
            GlossaryData glossaryData, 
            string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                XmlSerializer.Serialize(streamWriter, glossaryData);
            }
        }

        public void ExportDataToJson(
            GlossaryData glossaryData, 
            string fileName)
        {
            var jsonResolver = new ContractResolver();
            jsonResolver.IgnoreProperty(typeof(Record), "ObjectRecords");
            jsonResolver.IgnoreProperty(typeof(Record), "SubjectRecords");
            jsonResolver.IgnoreProperty(typeof(RecordAssociation), "SubjectRecord");
            jsonResolver.IgnoreProperty(typeof(RecordAssociation), "ObjectRecord");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = jsonResolver
            };

            var json = JsonConvert.SerializeObject(
                glossaryData,
                Formatting.Indented,
                settings);

            using var streamWriter = new StreamWriter(fileName);

            streamWriter.WriteLine(json);
        }

        public void ImportDataFromXML(
            string fileName, 
            out GlossaryData glossaryData)
        {
            using var streamReader = new StreamReader(fileName);

            glossaryData = XmlSerializer.Deserialize(streamReader) as GlossaryData;
        }

        public void ImportDataFromJson(
            string fileName, 
            out GlossaryData glossaryData)
        {
            using var streamReader = new StreamReader(fileName);

            var json = streamReader.ReadToEnd();
            glossaryData = JsonConvert.DeserializeObject<GlossaryData>(json);
        }
    }
}
