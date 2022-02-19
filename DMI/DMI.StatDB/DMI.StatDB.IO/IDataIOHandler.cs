using System.Collections.Generic;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            IList<Station> stationInformations,
            string fileName);

        void ExportDataToJson(
            IList<Station> stationInformations,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out IList<Station> stationInformations);

        void ImportDataFromJson(
            string fileName,
            out IList<Station> stationInformations);
    }
}
