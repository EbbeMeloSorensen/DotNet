using System.Collections.Generic;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            IList<Station> stations,
            IList<Position> positions,
            string fileName);

        void ExportDataToJson(
            IList<Station> stations,
            IList<Position> positions,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out IList<Station> stations,
            out IList<Position> positions);

        void ImportDataFromJson(
            string fileName,
            out IList<Station> stations,
            out IList<Position> positions);
    }
}
