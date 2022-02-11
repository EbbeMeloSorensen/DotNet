using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.IO
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
                xOver.Add(typeof(StationInformation), "SensorLocations", attrs);
                xOver.Add(typeof(StationInformation), "ContactPersons", attrs);
                xOver.Add(typeof(StationInformation), "LegalOwners", attrs);
                xOver.Add(typeof(StationInformation), "StationKeepers", attrs);
                xOver.Add(typeof(StationInformation), "MaintenanceRegulations", attrs);
                xOver.Add(typeof(StationInformation), "Errors", attrs);
                _xmlSerializer = new XmlSerializer(typeof(SMSData), xOver);

                return _xmlSerializer;
            }
        }

        public void ExportDataToXML(
            IList<StationInformation> stationInformations,
            string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                var smsData = new SMSData
                {
                    StationInformations = stationInformations.ToList()
                };

                XmlSerializer.Serialize(streamWriter, smsData);
            }
        }

        public void ExportDataToJson(
            IList<StationInformation> stationInformations,
            string fileName)
        {
            throw new NotImplementedException();
        }

        public void ImportDataFromXML(
            string fileName,
            out IList<StationInformation> stationInformations)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var contactData = XmlSerializer.Deserialize(streamReader) as SMSData;
                stationInformations = contactData.StationInformations;
            }
        }

        public void ImportDataFromJson(
            string fileName,
            out IList<StationInformation> stationInformations)
        {
            throw new NotImplementedException();
        }

        public List<DateTime> ReadObservationsForStationFromFile(
            string fileName)
        {
            var result = new List<DateTime>();

            using (var streamReader = new StreamReader(fileName))
            {
                string line;

                var dateOfObservation = DateTime.MinValue;

                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line == "No observations")
                    {
                        return result;
                    }

                    if (line.Length <= 10)
                    {
                        var elements = line.Split('-');
                        var year = int.Parse(elements[0]);
                        var month = int.Parse(elements[1]);
                        var day = int.Parse(elements[2]);

                        dateOfObservation = new DateTime(year, month, day);

                        continue;
                    }

                    var observationTimeAsString = line.Split()[1];
                    var hour = int.Parse(observationTimeAsString.Substring(0, 2));
                    var minute = int.Parse(observationTimeAsString.Substring(3, 2));
                    var second = int.Parse(observationTimeAsString.Substring(6, 2));

                    result.Add(new DateTime(
                        dateOfObservation.Year, 
                        dateOfObservation.Month,
                        dateOfObservation.Day,
                        hour,
                        minute,
                        second));
                }
            }

            return result;
        }

        public List<DateTime> ReadObservationsForStationFromDirectory(
            string directoryName,
            string searchPattern)
        {
            var result = new List<DateTime>();

            var directory = new DirectoryInfo(directoryName);

            var files = directory.GetFiles(searchPattern);

            foreach (var file in files)
            {
                var observationTimesForCurrentYear = 
                    ReadObservationsForStationFromFile(file.FullName);

                result.AddRange(observationTimesForCurrentYear);
            }

            result.Sort();

            return result;
        }

        public List<Tuple<DateTime, DateTime>> ConvertToIntervals(
            List<DateTime> observationTimes,
            double maxTolerableDifferenceBetweenTwoObservationsInDays)
        {
            var result = new List<Tuple<DateTime, DateTime>>();

            if (observationTimes.Count == 0)
            {
                return result;
            }

            var startOfCurrentInterval = observationTimes.First();

            var nObservations = observationTimes.Count;

            for (var i = 1; i < nObservations; i++)
            {
                var t1 = observationTimes[i - 1];
                var t2 = observationTimes[i];
                var diff = t2 - t1;

                if (diff.TotalDays > maxTolerableDifferenceBetweenTwoObservationsInDays)
                {
                    result.Add(new Tuple<DateTime, DateTime>(
                        startOfCurrentInterval, t1));

                    startOfCurrentInterval = t2;
                }
            }

            result.Add(new Tuple<DateTime, DateTime>(
                startOfCurrentInterval, observationTimes.Last()));

            return result;
        }

        public List<Tuple<DateTime, DateTime>> ReadObservationIntervalsForStation(
            string directoryName,
            string searchPattern,
            double maxTolerableDifferenceBetweenTwoObservationsInDays)
        {
            var fileName = Path.Combine(directoryName, "intervals.txt");
            var file = new FileInfo(fileName);

            List<Tuple<DateTime, DateTime>> result = null;

            if (file.Exists)
            {
                result = new List<Tuple<DateTime, DateTime>>();

                using (var streamReader = new StreamReader(fileName))
                {
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var year1 = int.Parse(line.Substring(0, 4));
                        var month1 = int.Parse(line.Substring(5, 2));
                        var day1 = int.Parse(line.Substring(8, 2));
                        var hour1 = int.Parse(line.Substring(11, 2));
                        var minute1 = int.Parse(line.Substring(14, 2));
                        var second1 = int.Parse(line.Substring(17, 2));

                        var year2 = int.Parse(line.Substring(22, 4));
                        var month2 = int.Parse(line.Substring(27, 2));
                        var day2 = int.Parse(line.Substring(30, 2));
                        var hour2 = int.Parse(line.Substring(33, 2));
                        var minute2 = int.Parse(line.Substring(36, 2));
                        var second2 = int.Parse(line.Substring(39, 2));

                        result.Add(new Tuple<DateTime, DateTime>(
                            new DateTime(year1, month1, day1, hour1, minute1, second1),
                            new DateTime(year2, month2, day2, hour2, minute2, second2)));
                    }

                    return result;
                }
            }

            var directory = new DirectoryInfo(directoryName);

            if (!directory.Exists)
            {
                return result;
            }

            var observationTimes = ReadObservationsForStationFromDirectory(
                directoryName,
                searchPattern);

            result = ConvertToIntervals(
                observationTimes,
                maxTolerableDifferenceBetweenTwoObservationsInDays);

            using (var streamWriter = new StreamWriter(fileName))
            {
                foreach (var interval in result)
                {
                    var t1 = interval.Item1;
                    var t2 = interval.Item2;

                    var year1 = $"{t1.Year}";
                    var month1 = $"{t1.Month}".PadLeft(2, '0');
                    var day1 = $"{t1.Day}".PadLeft(2, '0');
                    var hour1 = $"{t1.Hour}".PadLeft(2, '0');
                    var minute1 = $"{t1.Minute}".PadLeft(2, '0');
                    var second1 = $"{t1.Second}".PadLeft(2, '0');

                    var year2 = $"{t2.Year}";
                    var month2 = $"{t2.Month}".PadLeft(2, '0');
                    var day2 = $"{t2.Day}".PadLeft(2, '0');
                    var hour2 = $"{t2.Hour}".PadLeft(2, '0');
                    var minute2 = $"{t2.Minute}".PadLeft(2, '0');
                    var second2 = $"{t2.Second}".PadLeft(2, '0');

                    streamWriter.Write($"{year1}-");
                    streamWriter.Write($"{month1}-");
                    streamWriter.Write($"{day1} ");
                    streamWriter.Write($"{hour1}:");
                    streamWriter.Write($"{minute1}:");
                    streamWriter.Write($"{second1}");
                    streamWriter.Write(" - ");
                    streamWriter.Write($"{year2}-");
                    streamWriter.Write($"{month2}-");
                    streamWriter.Write($"{day2} ");
                    streamWriter.Write($"{hour2}:");
                    streamWriter.Write($"{minute2}:");
                    streamWriter.Write($"{second2}");
                    streamWriter.WriteLine();
                }
            }

            return result;
        }

        public void WriteStationHistoryToFile(
            List<StationInformation> stationHistory,
            string fileName)
        {
            var headerAsListOfStrings = stationHistory
                .First()
                .GenerateHeaderAsListOfString();

            var linesAsListsOfStrings = stationHistory
                .Select(s => s.AsListOfStrings())
                .ToList();

            var columnWidths = headerAsListOfStrings
                .Select(h => h.Length + 1)
                .ToList();

            linesAsListsOfStrings.ForEach(line =>
            {
                columnWidths = columnWidths
                    .Zip(line, (first, second) => System.Math.Max(first, second.Length + 1))
                    .ToList();
            });

            using (var streamWriter = new StreamWriter(fileName))
            {
                var header = Enumerable.Range(0, columnWidths.Count)
                    .Select(c => string.Format($"{{0,{-columnWidths[c]}}}", headerAsListOfStrings[c]))
                    .Aggregate((c, n) => c + ", " + n);

                streamWriter.WriteLine(header);

                linesAsListsOfStrings.ForEach(lineAsListsOfStrings =>
                {
                    var line = Enumerable.Range(0, columnWidths.Count)
                        .Select(c => string.Format($"{{0,{-columnWidths[c]}}}", lineAsListsOfStrings[c]))
                        .Aggregate((c, n) => c + ", " + n);

                    streamWriter.WriteLine(line);
                });
            }
        }
    }
}