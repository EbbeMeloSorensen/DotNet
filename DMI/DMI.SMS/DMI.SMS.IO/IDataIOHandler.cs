using System;
using System.Collections.Generic;
using System.IO;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.IO
{
    public interface IDataIOHandler
    {
        void ExportData(
            IList<StationInformation> stationInformations,
            string fileName);

        public void ImportData(
            string fileName,
            out IList<StationInformation> stationInformations);

        List<Tuple<DateTime, double>> ReadObservationsForStation(
            string nanoqStationId,
            string parameter,
            int firstYear,
            int lastYear);

        List<Tuple<DateTime, DateTime>> ReadObservationIntervalsForStation(
            string nanoqStationId,
            string parameter,
            double maxTolerableDifferenceBetweenTwoObservationsInDays);
    }
}
