using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Craft.IO.Utils;
using Glossary.Domain.Entities;
using Glossary.Domain.Foreign;
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
            GlossaryData prData, 
            string fileName)
        {
            var jsonResolver = new ContractResolver();
            jsonResolver.IgnoreProperty(typeof(Person), "ObjectPeople");
            jsonResolver.IgnoreProperty(typeof(Person), "SubjectPeople");
            jsonResolver.IgnoreProperty(typeof(PersonAssociation), "SubjectPerson");
            jsonResolver.IgnoreProperty(typeof(PersonAssociation), "ObjectPerson");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = jsonResolver
            };

            var json = JsonConvert.SerializeObject(
                prData,
                Formatting.Indented,
                settings);

            using var streamWriter = new StreamWriter(fileName);

            streamWriter.WriteLine(json);
        }

        public void ImportDataFromXML(
            string fileName, 
            out GlossaryData prData)
        {
            using var streamReader = new StreamReader(fileName);

            prData = XmlSerializer.Deserialize(streamReader) as GlossaryData;
        }

        public void ImportDataFromJson(
            string fileName, 
            out GlossaryData prData)
        {
            using var streamReader = new StreamReader(fileName);

            var json = streamReader.ReadToEnd();
            prData = JsonConvert.DeserializeObject<GlossaryData>(json);
        }

        public void ImportForeignDataFromJson(
            string fileName, 
            out ContactData contactData)
        {
            using var streamReader = new StreamReader(fileName);
            var json = streamReader.ReadToEnd();
            contactData = JsonConvert.DeserializeObject<ContactData>(json);
        }
    }
}
