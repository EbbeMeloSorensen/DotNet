using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using PR.Domain.Entities;

namespace PR.IO
{
    public class DataIOHandler : IDataIOHandler
    {
        private XmlSerializer _xmlSerializer;

        public void ExportDataToXML(IList<Person> people, string fileName)
        {
            throw new NotImplementedException();
        }

        public void ExportDataToJson(IList<Person> people, string fileName)
        {
            throw new NotImplementedException();
        }

        public void ImportDataFromXML(string fileName, out IList<Person> people)
        {
            throw new NotImplementedException();
        }

        public void ImportDataFromJson(string fileName, out IList<Person> people)
        {
            throw new NotImplementedException();
        }
    }
}
