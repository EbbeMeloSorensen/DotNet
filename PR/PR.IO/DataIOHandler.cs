using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Craft.IO.Utils;
using Newtonsoft.Json;
using PR.Domain.Foreign;
using Person = PR.Domain.Entities.Person;

namespace PR.IO
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
                _xmlSerializer = new XmlSerializer(typeof(PRData), xOver);

                return _xmlSerializer;
            }
        }

        public void ExportDataToXML(
            IList<Person> people, 
            string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                var smsData = new PRData
                {
                    People = people.ToList()
                };

                XmlSerializer.Serialize(streamWriter, smsData);
            }
        }

        public void ExportDataToJson(
            IList<Person> people, 
            string fileName)
        {
            var json = JsonConvert.SerializeObject(
                people, Formatting.Indented, new DoubleJsonConverter(), new NullableDoubleJsonConverter());

            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(json);
            }
        }

        public void ImportDataFromXML(
            string fileName, 
            out IList<Person> people)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var data = XmlSerializer.Deserialize(streamReader) as PRData;
                people = data.People;
            }
        }

        public void ImportDataFromJson(
            string fileName, 
            out IList<Person> people)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                people = JsonConvert.DeserializeObject<List<Person>>(json);
            }
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
