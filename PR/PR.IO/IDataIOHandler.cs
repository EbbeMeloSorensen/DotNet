using System.Collections.Generic;
using PR.Domain.Entities;

namespace PR.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            IList<Person> people,
            string fileName);

        void ExportDataToJson(
            IList<Person> people,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out IList<Person> people);

        void ImportDataFromJson(
            string fileName,
            out IList<Person> people);
    }
}
