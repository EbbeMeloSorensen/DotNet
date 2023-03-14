using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace DMI.DAL.ObsDB.UnitTest
{
    public class DataProviderTest : IDisposable
    {
        private string _host;
        private string _database;
        private string _obsDBUser;
        private string _obsDBPassword;

        public DataProviderTest()
        {
            //_host = "nanoq.dmi.dk";    // nanoq3, den operationelle database for observationer
            _host = "nanoq-ro.dmi.dk"; // nanoq4, dvs mirror databasen for nanoq3
            _database = "obsdb";

            // Sådan gør vi for Windows
            // var credentialsFilePath = Path.Combine(
            //     Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            //     @"Documents\credentialsForNanoq.txt");
            //var lines = File.ReadAllLines(credentialsFilePath);

            // Sådan gør vi for Linux. Filen skal ligge i samme folder som de kompilerede assemblies, dvs bin/Debug/net6.0
            var lines = File.ReadAllLines("credentialsForNanoq.txt");

            _obsDBUser = lines[0];
            _obsDBPassword = lines[1];
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task CheckConnection_GivenValidCredentials_ReturnsTrue()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(_host, _database, _obsDBUser, _obsDBPassword);

            // Assert
            connectionOK.Should().BeTrue();
        }

        [Fact]
        public async Task CheckConnection_GivenInvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(_host, _database, _obsDBUser, "wrongPassword");

            // Assert
            connectionOK.Should().BeFalse();
        }

        [Fact]
        public void CountObservationsOfIndividualParameterForStation_GivenThatDataProviderIsUninitialized_Throws()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var paramId = "temp_dry";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1954, 1, 1);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
            {
                dataProvider.CountObservationsOfIndividualParameterForStation(
                    _host,
                    _database,
                    _obsDBUser,
                    _obsDBPassword,
                    paramId,
                    stationId,
                    startTime,
                    endTime);
            });
        }

        [Fact]
        public void CountObservationsOfIndividualParameterForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var paramId = "temp_dry";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1954, 1, 1);

            dataProvider.Initialize(new[] { paramId });

            // Act
            var count = dataProvider.CountObservationsOfIndividualParameterForStation(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                paramId,
                stationId,
                startTime,
                endTime);

            // Assert
            count.Should().Be(2844);
        }

        [Fact]
        public void CountObservationsCoveredByBasisTableForStation_GivenThatDataProviderIsUninitialized_Throws()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1954, 1, 1);
            var includeZeroCountparams = false;

            // Act
            Assert.Throws<InvalidOperationException>(() =>
            {
                dataProvider.CountObservationsCoveredByBasisTableForStation(
                    _host,
                    _database,
                    _obsDBUser,
                    _obsDBPassword,
                    baseTableName,
                    stationId,
                    startTime,
                    endTime,
                    includeZeroCountparams);
            });
        }

        [Fact]
        public async Task CountObservationsCoveredByBasisTableForStation_GivenThatIncludeZeroCountParamsIsTrue_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1954, 1, 1);
            var includeZeroCountparams = true;

            dataProvider.Initialize(new[] {
                "temp_dry",
                "temp_dew",
                "wind_dir",
                "sun_last1h_glob"});

            // Act
            var result = await dataProvider.CountObservationsCoveredByBasisTableForStation(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime,
                includeZeroCountparams);

            // Assert
            result.Count.Should().Be(4);
            result["temp_dry"].Should().Be(2844);
            result["temp_dew"].Should().Be(2781);
            result["wind_dir"].Should().Be(2852);
            result["sun_last1h_glob"].Should().Be(0);
        }

        [Fact]
        public async Task CountObservationsCoveredByBasisTableForStation_GivenThatIncludeZeroCountParamsIsFalse_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1954, 1, 1);
            var includeZeroCountparams = false;

            dataProvider.Initialize(new[] {
                "temp_dry",
                "temp_dew",
                "wind_dir",
                "sun_last1h_glob"});

            // Act
            var result = await dataProvider.CountObservationsCoveredByBasisTableForStation(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime,
                includeZeroCountparams);

            // Assert
            result.Count.Should().Be(3);
            result["temp_dry"].Should().Be(2844);
            result["temp_dew"].Should().Be(2781);
            result["wind_dir"].Should().Be(2852);
        }

        [Fact]
        public async Task CountObservationsCoveredByBasisTableForStation_GivenThatBaseTableNameIsTempWindRadiation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var startTime = new DateTime(2019, 1, 1);
            var endTime = new DateTime(2019, 2, 1);
            var includeZeroCountparams = false;

            dataProvider.Initialize(new[] {
                "temp_dry",
                "temp_dew",
                "wind_dir",
                "sun_last1h_glob",
                "temp_max_past12h",
                "temp_min_past12h",
                "wind_max_per10min_past1h"
            });

            // Act
            var result = await dataProvider.CountObservationsCoveredByBasisTableForStation(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime,
                includeZeroCountparams);

            // Assert
            result.Count.Should().Be(7);
            result["temp_dry"].Should().Be(4464);
            result["temp_dew"].Should().Be(4464);
            result["wind_dir"].Should().Be(4464);
            result["temp_max_past12h"].Should().Be(62);
            result["temp_min_past12h"].Should().Be(62);
            result["sun_last1h_glob"].Should().Be(744);
            result["wind_max_per10min_past1h"].Should().Be(744);
        }

        [Fact]
        public void RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval_GivenThatDataProviderIsUninitialized_Throws()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1953, 1, 2);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
            {
                dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
                    _host,
                    _database,
                    _obsDBUser,
                    _obsDBPassword,
                    baseTableName,
                    stationId,
                    startTime,
                    endTime);
            });
        }

        [Fact]
        public void RetrieveObservationsCoveredByBasisTableForStationInGivenYear_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var year = 1953;

            dataProvider.Initialize(new[] {
                "temp_dry",
                "temp_dew",
                "wind_dir",
                "sun_last1h_glob"});

            // Act
            var observations = dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenYear(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                baseTableName,
                stationId,
                year);

            // Assert
            observations["temp_dry"].Count.Should().Be(2844);
            observations["temp_dew"].Count.Should().Be(2781);
            observations["wind_dir"].Count.Should().Be(2852);
            observations["sun_last1h_glob"].Count.Should().Be(0);
        }

        [Fact]
        public void DumpObservationFilesForTempDryForSelectedStations()
        {
            try
            {
                // This is not really a unit test but rather a means of dumping some data for use in another tool
                var dataProvider = new DataProvider(null);
                dataProvider.Initialize(new[] { "temp_dry" });
                var baseTableName = "temp_wind_radiation";

                var stationIds = new List<string>
                {
                    //"06031",
                    "06032",
                    //"06041",
                    "06049",
                    "06051",
                    "06052",
                    "06056",
                    "06058",
                    "06065",
                    "06068",
                    "06072",
                    "06073",
                    "06074",
                    "06079",
                    "06081",
                    "06082",
                    "06088",
                    "06093",
                    "06096",
                    "06102",
                    "06116",
                    "06119",
                    "06123",
                    "06124",
                    "06126",
                    "06132",
                    "06135",
                    "06136",
                    "06138",
                    "06141",
                    "06147",
                    "06149",
                    "06151",
                    "06154",
                    "06156",
                    "06159",
                    "06168",
                    "06169",
                    "06174",
                    "06181",
                    "06183",
                    "06184",
                    "06186",
                    "06187",
                    "06188",
                    "06193",
                    "06197"
                };

                var years = Enumerable.Range(1953, 69).ToList();

                foreach (var stationId in stationIds)
                {
                    var directoryName = Path.Combine(@"C:\Data\Observations", $"{stationId}");
                    var directory = new DirectoryInfo(directoryName);

                    if (!directory.Exists)
                    {
                        directory.Create();
                    }

                    foreach (var year in years)
                    {
                        var fileName = Path.Combine(directoryName, $"temp_dry_{stationId}_{year}.txt");

                        var file = new FileInfo(fileName);

                        if (file.Exists)
                        {
                            continue;
                        }

                        using (var streamWriter = new StreamWriter(fileName))
                        {
                            Dictionary<string, List<Tuple<DateTime, float>>> observations = null;
                            var failedAttempts = 0;

                            while (failedAttempts < 10)
                            {
                                try
                                {
                                    observations = dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenYear(
                                        _host,
                                        _database,
                                        _obsDBUser,
                                        _obsDBPassword,
                                        baseTableName,
                                        stationId,
                                        year);

                                    break;
                                }
                                catch (Exception)
                                {
                                    failedAttempts++;
                                    Thread.Sleep(2000);
                                }
                            }

                            if (failedAttempts == 10)
                            {
                                throw new InvalidDataException();
                            }

                            var currentDayOfYear = 0;

                            if (observations == null || observations.First().Value.Count == 0)
                            {
                                streamWriter.WriteLine($"No observations");
                                continue;
                            }

                            foreach (var kvp in observations)
                            {
                                foreach (var observation in kvp.Value)
                                {
                                    var time = observation.Item1;
                                    var value = observation.Item2;

                                    if (currentDayOfYear != time.DayOfYear)
                                    {
                                        currentDayOfYear = time.DayOfYear;
                                        streamWriter.WriteLine($"{time.Year}-{time.Month}-{time.Day}");
                                    }

                                    var hour = time.Hour.ToString().PadLeft(2, '0');
                                    var minute = time.Minute.ToString().PadLeft(2, '0');
                                    var second = time.Second.ToString().PadLeft(2, '0');

                                    streamWriter.WriteLine($" {hour}:{minute}:{second} {value}");
                                }
                            }

                            streamWriter.Close();
                        }
                    }
                };
            }
            catch (Exception e2)
            {
                throw e2;
            }
        }

        [Fact]
        public void RetrieveObservationsForStationInGivenYear_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "06041";
            var year = 1953;

            // Todo: fyld nogle flere ind her
            dataProvider.Initialize(new[] {
                "temp_dry",
                "temp_dew",
                "wind_dir",
                "sun_last1h_glob"});

            // Act
            var observations = dataProvider.RetrieveObservationsForStationInGivenYear(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                stationId,
                year);

            // Assert
        }

        [Fact]
        public async Task RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "temp_wind_radiation";
            var stationId = "06041";
            var startTime = new DateTime(1953, 1, 1);
            var endTime = new DateTime(1953, 1, 2);

            dataProvider.Initialize(new[] {
                "temp_dry",
                "temp_dew",
                "wind_dir",
                "sun_last1h_glob"});

            // Act
            var observations = await dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
                _host,
                _database,
                _obsDBUser,
                _obsDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime);

            // Assert
            observations["temp_dry"].Count.Should().Be(7);
            observations["temp_dew"].Count.Should().Be(0);
            observations["wind_dir"].Count.Should().Be(7);
            observations["sun_last1h_glob"].Count.Should().Be(0);

            observations["temp_dry"]
                .Select(o => o.Item1.Hour)
                .SequenceEqual(new[] { 0, 6, 9, 12, 15, 18, 21 })
                .Should().BeTrue();

            observations["temp_dry"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 1.0f, 0, 0, 1, 0, 0, 0 })
                .Should().BeTrue();

            observations["wind_dir"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 180.0f, 140, 110, 140, 140, 110, 110 })
                .Should().BeTrue();
        }

        [Fact]
        public async Task IdentifyTimeForOldestObservationForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "06041";

            // Act
            var time = await dataProvider.IdentifyTimeForOldestObservationForStation(
                _host, _database, _obsDBUser, _obsDBPassword, stationId);

            // Assert
            time.HasValue.Should().BeTrue();
            time.Value.Year.Should().Be(1953);
            time.Value.Month.Should().Be(1);
            time.Value.Day.Should().Be(1);
            time.Value.Hour.Should().Be(0);
            time.Value.Minute.Should().Be(0);
            time.Value.Second.Should().Be(0);
        }

        [Fact]
        public async Task IdentifyTimeForMostRecentObservationForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "04203";

            // Act
            var time = await dataProvider.IdentifyTimeForMostRecentObservationForStation(
                _host, _database, _obsDBUser, _obsDBPassword, stationId);

            // Assert
            time.HasValue.Should().BeTrue();
            time.Value.Year.Should().Be(DateTime.Now.Year);
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredBySpecificTableForStation_GivenYearExists_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var basisTableName = "precip_hum_pressure";
            var year = 1953;
            var stationId = "06041";

            // Act
            var time = dataProvider.IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
                _host, _database, _obsDBUser, _obsDBPassword, basisTableName, year, stationId, false);

            // Assert
            time.HasValue.Should().BeTrue();
            time.Value.Year.Should().Be(1953);
            time.Value.Month.Should().Be(1);
            time.Value.Day.Should().Be(1);
            time.Value.Hour.Should().Be(0);
            time.Value.Minute.Should().Be(0);
            time.Value.Second.Should().Be(0);
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredBySpecificTableForStation_GivenYearDoesntExist_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var basisTableName = "temp_wind_radiation_1";
            var year = 1957;
            var stationId = "06122";

            // Act
            var time = dataProvider.IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
                _host, _database, _obsDBUser, _obsDBPassword, basisTableName, year, stationId, false);

            // Assert
            time.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task IdentifyTimeForOldestObservationCoveredByBasisTableForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var basisTableName = "temp_wind_radiation_1";
            var stationId = "06122";

            // Act
            var time = await dataProvider.IdentifyTimeForOldestObservationCoveredByBasisTableForStation(
                _host, _database, _obsDBUser, _obsDBPassword, basisTableName, stationId);

            // Assert
            time.HasValue.Should().BeTrue();
            time.Value.Year.Should().Be(1961);
            time.Value.Month.Should().Be(1);
            time.Value.Day.Should().Be(1);
            time.Value.Hour.Should().Be(6);
            time.Value.Minute.Should().Be(0);
            time.Value.Second.Should().Be(0);
        }

        [Fact]
        public async Task IdentifyTimeForMostRecentObservationCoveredByBasisTableForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var basisTableName = "temp_wind_radiation_1";
            var stationId = "06068";

            // Act
            var time = await dataProvider.IdentifyTimeForMostRecentObservationCoveredByBasisTableForStation(
                _host, _database, _obsDBUser, _obsDBPassword, basisTableName, stationId);

            // Assert
            time.HasValue.Should().BeTrue();
            time.Value.Year.Should().BeGreaterOrEqualTo(2019); // It is 2019 for nanoq4
        }
    }
}
