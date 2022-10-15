using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            PRData prData,
            string fileName);

        void ExportDataToJson(
            PRData prData,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out PRData prData);

        void ImportDataFromJson(
            string fileName,
            out PRData prData);

        void ImportForeignDataFromJson(
            string fileName,
            out Domain.Foreign.ContactData contactData);
    }
}
