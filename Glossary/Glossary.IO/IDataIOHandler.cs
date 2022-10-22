using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            GlossaryData glossaryData,
            string fileName);

        void ExportDataToJson(
            GlossaryData glossaryData,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out GlossaryData glossaryData);

        void ImportDataFromJson(
            string fileName,
            out GlossaryData glossaryData);
    }
}
