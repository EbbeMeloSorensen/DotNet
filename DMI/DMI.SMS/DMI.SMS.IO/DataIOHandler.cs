﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Craft.IO.Utils;
using DMI.SMS.Domain.Entities;
using Newtonsoft.Json;

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

        public void ExportData(
            IList<StationInformation> stationInformations,
            string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            switch (extension)
            {
                case ".xml":
                {
                    ExportDataToXML(stationInformations, fileName);
                    break;
                }
                case ".json":
                {
                    ExportDataToJson(stationInformations, fileName);
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }

        public void ImportData(
            string fileName,
            out IList<StationInformation> stationInformations)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            switch (extension)
            {
                case ".xml":
                {
                    ImportDataFromXML(fileName, out stationInformations);
                    break;
                }
                case ".json":
                {
                    ImportDataFromJson(fileName, out stationInformations);
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }

        private void ImportDataFromXML(
            string fileName,
            out IList<StationInformation> stationInformations)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var contactData = XmlSerializer.Deserialize(streamReader) as SMSData;
                stationInformations = contactData.StationInformations;
            }
        }

        private void ImportDataFromJson(
            string fileName,
            out IList<StationInformation> stationInformations)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                stationInformations = JsonConvert.DeserializeObject<List<StationInformation>>(json);
            }
        }

        public List<Tuple<DateTime, double>> ReadObservationsForStationFromFile(
            string fileName)
        {
            var result = new List<Tuple<DateTime, double>>();

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

                    var lineElements = line.Split();
                    var observationTimeAsString = lineElements[1];
                    var observationValueAsString = lineElements[2];
                    var hour = int.Parse(observationTimeAsString.Substring(0, 2));
                    var minute = int.Parse(observationTimeAsString.Substring(3, 2));
                    var second = int.Parse(observationTimeAsString.Substring(6, 2));
                    var value = double.Parse(observationValueAsString, NumberStyles.Number, CultureInfo.InvariantCulture);

                    result.Add(new Tuple<DateTime, double>(
                        new DateTime(
                        dateOfObservation.Year, 
                        dateOfObservation.Month,
                        dateOfObservation.Day,
                        hour,
                        minute,
                        second),
                        value));
                }
            }

            return result;
        }

        public List<Tuple<DateTime, double>> ReadObservationsForStation(
            string nanoqStationId,
            string parameter,
            int firstYear,
            int lastYear)
        {
            var result = new List<Tuple<DateTime, double>>();
            var years = Enumerable.Range(firstYear, lastYear - firstYear + 1);

            foreach (var year in years)
            {
                var directory = new DirectoryInfo(Path.Combine(@"C:\Data\Observations", $"{year}", $"{nanoqStationId}"));

                if (!directory.Exists)
                {
                    continue;
                }

                var fileName = $"{nanoqStationId}_{parameter}_{year}.txt";
                var file = directory.GetFiles(fileName).SingleOrDefault();

                if (file == null)
                {
                    continue;
                }

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

        // Først leder den efter en fil ved navn intervals.txt i den folder, der svarer til stationen.
        // Hvis ikke den er der, så leder den i samme folder efter de rå observationer. Der skulle gerne ligge en fil pr år.
        // Disse filer skal vel at mærke være genereret af et andet program, hvis det skal virke. Hvis ikke det er gjort, så
        // får man bare ikke noget tilbage.
        // Det kunne være en fin opgave at lade den GENERERE de filer, hvis den har adgang til databasen
        public List<Tuple<DateTime, DateTime>> ReadObservationIntervalsForStation(
            string nanoqStationId,
            string parameter,
            double maxTolerableDifferenceBetweenTwoObservationsInDays)
        {
            var fileName = Path.Combine(@"C:\Data\Stations", $"{nanoqStationId}", "intervals.txt");
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

            var observations = ReadObservationsForStation(
                nanoqStationId,
                parameter,
                1953,
                DateTime.UtcNow.Year);

            result = ConvertToIntervals(
                observations.Select(_ => _.Item1).ToList(),
                maxTolerableDifferenceBetweenTwoObservationsInDays);

            if (!Directory.Exists(@"C:\Data\Stations"))
            {
                Directory.CreateDirectory(@"C:\Data\Stations");
            }

            var stationDirectory = Path.Combine(@"C:\Data\Stations", nanoqStationId);

            if (!Directory.Exists(stationDirectory))
            {
                Directory.CreateDirectory(stationDirectory);
            }

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

        private void ExportDataToXML(
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

        private void ExportDataToJson(
            IList<StationInformation> stationInformations,
            string fileName)
        {
            var json = JsonConvert.SerializeObject(
                stationInformations, Formatting.Indented, new DoubleJsonConverter(), new NullableDoubleJsonConverter());

            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(json);
            }
        }
    }
}