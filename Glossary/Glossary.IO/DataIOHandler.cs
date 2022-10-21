using System.IO;
using System.Xml.Serialization;
using Craft.IO.Utils;
using Newtonsoft.Json;
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
                xOver.Add(typeof(Person), "ObjectPeople", attrs);
                xOver.Add(typeof(Person), "SubjectPeople", attrs);
                xOver.Add(typeof(PersonAssociation), "SubjectPerson", attrs);
                xOver.Add(typeof(PersonAssociation), "ObjectPerson", attrs);
                _xmlSerializer = new XmlSerializer(typeof(PRData), xOver);

                return _xmlSerializer;
            }
        }

        public void ExportDataToXML(
            PRData prData, 
            string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                XmlSerializer.Serialize(streamWriter, prData);
            }
        }

        public void ExportDataToJson(
            PRData prData, 
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
            out PRData prData)
        {
            using var streamReader = new StreamReader(fileName);

            prData = XmlSerializer.Deserialize(streamReader) as PRData;
        }

        public void ImportDataFromJson(
            string fileName, 
            out PRData prData)
        {
            using var streamReader = new StreamReader(fileName);

            var json = streamReader.ReadToEnd();
            prData = JsonConvert.DeserializeObject<PRData>(json);
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
