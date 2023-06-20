using System;
using System.Collections.Generic;
using System.IO;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.IO
{
    public interface IDataIOHandler
    {
        void ExportDataToXML(
            IList<StationInformation> stationInformations,
            string fileName);

        void ExportDataToJson(
            IList<StationInformation> stationInformations,
            string fileName);

        void ImportDataFromXML(
            string fileName,
            out IList<StationInformation> stationInformations);

        void ImportDataFromJson(
            string fileName,
            out IList<StationInformation> stationInformations);

        List<Tuple<DateTime, double>> ReadObservationsForStation(
            string directoryName,
            string searchPattern);

        List<Tuple<DateTime, DateTime>> ReadObservationIntervalsForStation(
            string directoryName,
            string searchPattern,
            double maxTolerableDifferenceBetweenTwoObservationsInDays);
    }
}
