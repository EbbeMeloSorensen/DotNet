using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            GlossaryData prData,
            string fileName);

        void ExportDataToJson(
            GlossaryData prData,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out GlossaryData prData);

        void ImportDataFromJson(
            string fileName,
            out GlossaryData prData);

        void ImportForeignDataFromJson(
            string fileName,
            out Domain.Foreign.ContactData contactData);
    }
}
