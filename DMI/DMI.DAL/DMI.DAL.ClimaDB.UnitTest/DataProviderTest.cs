using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMI.Utils;
using FluentAssertions;
using Xunit;

namespace DMI.DAL.ClimaDB.UnitTest
{
    public class DataProviderTest
    {
        private List<string> _climateStations = new List<string>
        {
            "04203",
            "04208",
            "04214",
            "04220",
            "04228",
            "04242",
            "04250",
            "04253",
            "04266",
            "04271",
            "04272",
            "04285",
            "04301",
            "04312",
            "04313",
            "04320",
            "04330",
            "04339",
            "04351",
            "04360",
            "04373",
            "04382",
            "04390",
            "05005",
            "05009",
            "05015",
            "05031",
            "05035",
            "05042",
            "05065",
            "05070",
            "05075",
            "05081",
            "05085",
            "05089",
            "05095",
            "05105",
            "05109",
            "05135",
            "05140",
            "05150",
            "05160",
            "05165",
            "05169",
            "05185",
            "05199",
            "05202",
            "05205",
            "05220",
            "05225",
            "05269",
            "05272",
            "05276",
            "05277",
            "05290",
            "05296",
            "05300",
            "05305",
            "05320",
            "05329",
            "05343",
            "05345",
            "05350",
            "05355",
            "05365",
            "05375",
            "05381",
            "05395",
            "05400",
            "05406",
            "05408",
            "05435",
            "05440",
            "05450",
            "05455",
            "05469",
            "05499",
            "05505",
            "05510",
            "05529",
            "05537",
            "05545",
            "05575",
            "05735",
            "05880",
            "05889",
            "05935",
            "05945",
            "05970",
            "05986",
            "05994",
            "06019",
            "06031",
            "06032",
            "06041",
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
            "06197",
            "20000",
            "20030",
            "20055",
            "20085",
            "20228",
            "20279",
            "20315",
            "20375",
            "20400",
            "20552",
            "20561",
            "20600",
            "20670",
            "21020",
            "21080",
            "21100",
            "21120",
            "21160",
            "21208",
            "21368",
            "21430",
            "22020",
            "22080",
            "22162",
            "22189",
            "22232",
            "22410",
            "23100",
            "23133",
            "23160",
            "23327",
            "23360",
            "24043",
            "24102",
            "24142",
            "24171",
            "24380",
            "24430",
            "24490",
            "25045",
            "25161",
            "25270",
            "25339",
            "26210",
            "26340",
            "26358",
            "26450",
            "27008",
            "27082",
            "28032",
            "28110",
            "28240",
            "28280",
            "28385",
            "28552",
            "28590",
            "29020",
            "29194",
            "29243",
            "29330",
            "29440",
            "30075",
            "30187",
            "30215",
            "30414",
            "31040",
            "31185",
            "31199",
            "31259",
            "31350",
            "31400",
            "31509",
            "31570",
            "32110",
            "32175",
            "34270",
            "34320",
            "34339"
        };

        private Dictionary<int, string> _elem_no_map = new Dictionary<int, string>
        {
            { 101, "mean_temp" },
            { 111, "mean_daily_max_temp" },
            { 112, "max_temp_w_date" },
            { 113, "max_temp_12h" },
            { 114, "no_ice_days" },
            { 115, "no_summer_days" },
            { 121, "mean_daily_min_temp" },
            { 122, "min_temp" },
            { 123, "min_temperature_12h" },
            { 124, "no_cold_days" },
            { 125, "no_frost_days" },
            { 126, "no_tropical_nights" },
            { 147, "acc_heating_degree_days_17" },
            { 149, "acc_heating_degree_days_19" },
            { 201, "mean_relative_hum" },
            { 205, "max_relative_hum" },
            { 207, "min_relative_hum" },
            { 210, "mean_vapour_pressure" },
            { 301, "mean_wind_speed" },
            { 302, "max_wind_speed_10min" },
            { 305, "max_wind_speed_3sec" },
            { 311, "no_windy_days" },
            { 321, "no_stormy_days" },
            { 326, "no_days_w_storm" },
            { 331, "no_days_w_hurricane" },
            { 365, "mean_wind_dir_min0" },
            { 371, "mean_wind_dir" },
            { 401, "mean_pressure" },
            { 410, "max_pressure" },
            { 420, "min_pressure" },
            { 504, "bright_sunshine" },
            { 550, "mean_radiation" },
            { 601, "acc_precip" },
            { 602, "max_precip_24h" },
            { 603, "acc_precip_past12h" },
            { 604, "no_days_acc_precip_01" },
            { 605, "no_days_acc_precip_1" },
            { 606, "no_days_acc_precip_10" },
            { 609, "acc_precip_past24h" },
            { 633, "max_precip_30m" },
            { 701, "no_days_snow_cover" },
            { 801, "mean_cloud_cover" },
            { 802, "no_clear_days" },
            { 803, "no_cloudy_days" },
            { 902, "max_snow_depth" },
            { 906, "snow_depth" },
            { 910, "snow_cover" },
            { 106, "temp_grass" },
            { 107, "temp_soil_10" },
            { 108, "temp_soil_30" },
            { 213, "leaf_moisture" },
            { 251, "vapour_pressure_deficit_mean" },
        };

        private string _host;
        private string _database;
        private string _climaDBUser;
        private string _climaDBPassword;

        public DataProviderTest()
        {
            //_host = "nanoq.dmi.dk";    // nanoq3, den operationelle database for observationer
            _host = "nanoq-ro.dmi.dk"; // nanoq4, dvs mirror databasen for nanoq3
            _database = "climadb";

            // Sådan gør vi for Windows
            // var credentialsFilePath = Path.Combine(
            //     Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            //     @"Documents\credentialsForNanoq.txt");
            //var lines = File.ReadAllLines(credentialsFilePath);

            // Sådan gør vi for Linux. Filen skal ligge i samme folder som de kompilerede assemblies, dvs bin/Debug/net6.0
            var lines = File.ReadAllLines("credentialsForNanoq.txt");

            _climaDBUser = lines[0];
            _climaDBPassword = lines[1];
        }

        [Fact]
        public async Task CheckConnection_GivenValidCredentials_ReturnsTrue()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(_host, _database, _climaDBUser, _climaDBPassword);

            // Assert
            connectionOK.Should().BeTrue();
        }

        [Fact]
        public async Task CheckConnection_GivenInvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(_host, _database, _climaDBUser, "wrongPassword");

            // Assert
            connectionOK.Should().BeFalse();
        }

        [Fact]
        public async Task CountAllManualPrecipitationObservationsForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "34270";
            var startTime = new DateTime(2009, 1, 1);
            var endTime = new DateTime(2010, 1, 1);

            // Act
            var result = await dataProvider.CountAllManualPrecipitationObservationsForStation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                startTime,
                endTime);

            // Assert
            result.Should().Be(344);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation6019AndElemNo101_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "601900";
            int elem_no = 101;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(2001);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation4203AndElemNo101_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "420300";
            int elem_no = 101;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(1974);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation4203AndElemNo113_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "420300";
            int elem_no = 113;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(1974);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation6065AndElemNo251_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "606500";
            int elem_no = 251;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(2002);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation6019AndElemNo213_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "601900";
            int elem_no = 213;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(2011);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation22080AndElemNo906_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "2208050";
            int elem_no = 906;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(2011);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation22080AndElemNo602_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "2208050";
            int elem_no = 602;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(2001);
        }

        [Fact]
        public async Task IdentifyYearForFirstObservation_GivenStation4203AndElemNo601_ReturnsCorrectYear()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            string stationId = "0420300";
            int elem_no = 601;

            // Act
            var result = await dataProvider.IdentifyYearForFirstObservation(
                _host,
                _database,
                _climaDBUser,
                _climaDBPassword,
                stationId,
                elem_no,
                null,
                null);

            // Assert
            //result.HasValue.Should().BeTrue();
            //result.Value.Should().Be(2001);
        }

        [Fact]
        public async Task GenerateObservationMatrixForClimateParameters()
        {
            var dataProvider = new DataProvider(null);

            short minYearDK = 2011;
            short minYearNA = 2014;

            var paramNamesSorted = _elem_no_map.Values
                .OrderBy(p => p)
                .ToList();

            var fileName = Path.Combine(@"C:\Temp\", $"ClimateDataOverview.txt");

            using (var streamWriter = new StreamWriter(fileName))
            {
                var header =
                    string.Format($"{{0,{-10}}}", "Station") +
                    ", " + paramNamesSorted
                        .Select(p => $"{p,27}")
                        .Aggregate((c, n) => $"{c}, {n}");

                streamWriter.WriteLine(header);

                foreach (var stationId in _climateStations)
                {
                    var yearsForCurrentStation = new List<string>();

                    foreach (var paramName in paramNamesSorted)
                    {
                        var paramCode = _elem_no_map.FirstOrDefault(kvp => kvp.Value == paramName).Key;

                        var year = await dataProvider.IdentifyYearForFirstObservation(
                            _host,
                            _database,
                            _climaDBUser,
                            _climaDBPassword,
                            stationId.AsNanoqStationId(),
                            paramCode,
                            minYearDK,
                            minYearNA);

                        if (year.HasValue)
                        {
                            yearsForCurrentStation.Add(year.ToString());
                        }
                        else
                        {
                            yearsForCurrentStation.Add("na");
                        }
                    }

                    var line =
                        string.Format($"{{0,{-10}}}", stationId) +
                        ", " + yearsForCurrentStation
                                .Select(year => $"{year,27}")
                                .Aggregate((c, n) => $"{c}, {n}");

                    streamWriter.WriteLine(line);
                };
            }
        }
    }
}