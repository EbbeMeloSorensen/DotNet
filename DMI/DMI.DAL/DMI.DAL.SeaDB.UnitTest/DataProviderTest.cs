using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace DMI.DAL.SeaDB.UnitTest
{
    public class DataProviderTest
    {
        //private const string host = "nanoq.dmi.dk"; // nanoq4, den operationelle database for observationer
        private const string _host = "nanoq-ro.dmi.dk"; // nanoq4, dvs mirror databasen for nanoq3
        private const string _database = "seadb";
        private const string _seaDBUser = "ebs";
        private const string _seaDBPassword = "yS5Luf8k";

        [Fact]
        public async Task CheckConnection_GivenValidCredentials_ReturnsTrue()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(_host, _database, _seaDBUser, _seaDBPassword);

            // Assert
            connectionOK.Should().BeTrue();
        }

        [Fact]
        public async Task CheckConnection_GivenInvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(_host, _database, _seaDBUser, "wrongPassword");

            // Assert
            connectionOK.Should().BeFalse();
        }

        [Fact]
        public async Task CountAllObservationsForStation_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "20003";
            var startTime = new DateTime(2003, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2003, 1, 1, 1, 0, 0);
            var includeZeroCountParams = true;

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var result = await dataProvider.CountAllObservationsForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                stationId,
                startTime,
                endTime,
                includeZeroCountParams);

            // Assert
            result["tw"].Should().Be(0);
            result["sea_reg"].Should().Be(6);
            result["sealev_ln"].Should().Be(0);
            result["sealev_dvr"].Should().Be(0);
        }

        [Fact]
        public void CountObservationsCoveredByBasisTableForStation_GivenThatIncludeZeroCountParamsIsTrue_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sealev";
            var stationId = "20003";
            var startTime = new DateTime(2003, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2003, 1, 1, 1, 0, 0);
            var includeZeroCountParams = true;

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var result = dataProvider.CountObservationsCoveredByBasisTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime,
                includeZeroCountParams);

            // Assert
            result["sea_reg"].Should().Be(6);
            result["sealev_ln"].Should().Be(0);
            result["sealev_dvr"].Should().Be(0);
        }

        [Fact]
        public void CountObservationsCoveredByBasisTableForStation_GivenThatIncludeZeroCountParamsIsFalse_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sealev";
            var stationId = "20003";
            var startTime = new DateTime(2003, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2003, 1, 1, 1, 0, 0);
            var includeZeroCountParams = false;

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var result = dataProvider.CountObservationsCoveredByBasisTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime,
                includeZeroCountParams);

            // Assert
            result.Count().Should().Be(1);
            result["sea_reg"].Should().Be(6);
        }

        [Fact]
        public void CountObservationsCoveredByBasisTableForStation_GivenThatBasisTableIsSeaTempSalt_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sea_temp_salt";
            var stationId = "24032";
            var startTime = new DateTime(2003, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2003, 1, 1, 1, 0, 0);
            var includeZeroCountParams = true;

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var result = dataProvider.CountObservationsCoveredByBasisTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime,
                includeZeroCountParams);

            // Assert
            result.Count().Should().Be(1);
            result["tw"].Should().Be(6);
        }

        [Fact]
        public async Task RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval_GivenThatBasisTableIsSeaTempSalt_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sea_temp_salt";
            var stationId = "24032";
            var startTime = new DateTime(2003, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2003, 1, 1, 1, 0, 0);

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var observations = await dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime);

            // Assert
            observations["tw"].Count.Should().Be(6);

            observations["tw"]
                .Select(o => o.Item1.Minute)
                .SequenceEqual(new[] { 0, 10, 20, 30, 40, 50 })
                .Should().BeTrue();

            observations["tw"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 1.9f, 1.8f, 1.6f, 1.5f, 1.5f, 1.5f })
                .Should().BeTrue();
        }

        [Fact]
        public async Task RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval_GivenThatBasisTableIsSeaLevAndDecadeStarts2010_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sealev";
            var stationId = "20101";
            var startTime = new DateTime(2013, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2013, 1, 1, 1, 0, 0);

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var observations = await dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime);

            // Assert
            observations["sea_reg"].Count.Should().Be(6);
            observations["sealev_ln"].Count.Should().Be(6);
            observations["sealev_dvr"].Count.Should().Be(6);

            observations["sea_reg"]
                .Select(o => o.Item1.Minute)
                .SequenceEqual(new[] { 0, 10, 20, 30, 40, 50 })
                .Should().BeTrue();

            observations["sealev_ln"]
                .Select(o => o.Item1.Minute)
                .SequenceEqual(new[] { 0, 10, 20, 30, 40, 50 })
                .Should().BeTrue();

            observations["sealev_dvr"]
                .Select(o => o.Item1.Minute)
                .SequenceEqual(new[] { 0, 10, 20, 30, 40, 50 })
                .Should().BeTrue();

            observations["sea_reg"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { -7f, -6, -6, -6, -6, -6 })
                .Should().BeTrue();

            observations["sealev_ln"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { -4f, -3, -3, -3, -3, -3 })
                .Should().BeTrue();

            observations["sealev_dvr"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { -8f, -7, -7, -7, -7, -7 })
                .Should().BeTrue();
        }

        [Fact]
        public async Task RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval_GivenThatBasisTableIsSeaLevAndDecadeStarts1980_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sealev";
            var stationId = "21008";
            var startTime = new DateTime(1983, 1, 1, 0, 0, 0);
            var endTime = new DateTime(1983, 1, 1, 1, 0, 0);

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var observations = await dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime);

            // Assert
            observations["sea_reg"].Count.Should().Be(4);
            observations["sealev_ln"].Count.Should().Be(4);

            observations["sea_reg"]
                .Select(o => o.Item1.Minute)
                .SequenceEqual(new[] { 0, 15, 30, 45 })
                .Should().BeTrue();

            observations["sealev_ln"]
                .Select(o => o.Item1.Minute)
                .SequenceEqual(new[] { 0, 15, 30, 45 })
                .Should().BeTrue();

            observations["sea_reg"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 499f, 497, 500, 504 })
                .Should().BeTrue();

            observations["sealev_ln"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 15f, 13, 16, 20 })
                .Should().BeTrue();
        }

        [Fact]
        public async Task RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval_GivenThatBasisTableIsSeaLevAndDecadeStarts1910_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sealev";
            var stationId = "25148";
            var startTime = new DateTime(1912, 1, 1, 0, 0, 0);
            var endTime = new DateTime(1912, 1, 1, 4, 0, 0);

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var observations = await dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                startTime,
                endTime);

            // Assert
            observations["sea_reg"].Count.Should().Be(4);
            observations["sealev_ln"].Count.Should().Be(4);

            observations["sea_reg"]
                .Select(o => o.Item1.Hour)
                .SequenceEqual(new[] { 0, 1, 2, 3 })
                .Should().BeTrue();

            observations["sealev_ln"]
                .Select(o => o.Item1.Hour)
                .SequenceEqual(new[] { 0, 1, 2, 3 })
                .Should().BeTrue();

            observations["sea_reg"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 285f, 261, 229, 200 })
                .Should().BeTrue();

            observations["sealev_ln"]
                .Select(o => o.Item2)
                .SequenceEqual(new[] { 37f, 13, -19, -48 })
                .Should().BeTrue();
        }

        [Fact]
        public void RetrieveObservationsCoveredByBasisTableForStationInGivenYear_GivenThatBasisTableIsSeaTempSalt_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var baseTableName = "sea_temp_salt";
            var stationId = "24032";
            var year = 2003;

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var observations = dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenYear(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                baseTableName,
                stationId,
                year);

            // Assert
            observations["tw"].Count.Should().BeGreaterThan(980000);
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaTempSaltAndStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sea_temp_salt";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                false);

            // Assert
            time.HasValue.Should().BeFalse();
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaLev_1878_69AndStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sealev_1878_69";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                false);

            // Assert
            time.HasValue.Should().BeFalse();
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaLev_1970_79AndStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sealev_1970_79";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                false);

            // Assert
            time.Value.Year.Should().Be(1971);
            time.Value.Month.Should().Be(11);
            time.Value.Day.Should().Be(4);
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaLev_1980_89AndStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sealev_1980_89";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                false);

            // Assert
            time.Value.Year.Should().Be(1980);
            time.Value.Month.Should().Be(1);
            time.Value.Day.Should().Be(1);
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaLev_1990_99AndStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sealev_1990_99";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                false);

            // Assert
            time.Value.Year.Should().Be(1990);
            time.Value.Month.Should().Be(1);
            time.Value.Day.Should().Be(1);
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaLev_2000_09AndStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sealev_2000_09";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                false);

            // Assert
            time.HasValue.Should().BeFalse();
        }

        [Fact]
        public void IdentifyTimeForOldestObservationCoveredByTableForStation_GivenTableSeaLev_1990_99AndStation20048AndNewest_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var tableName = "sealev_1990_99";
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = dataProvider.IdentifyTimeForOldestObservationCoveredByTableForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                tableName,
                stationId,
                true);

            // Assert
            time.Value.Year.Should().Be(1991);
            time.Value.Month.Should().Be(11);
            time.Value.Day.Should().Be(12);
        }

        [Fact]
        public async Task IdentifyTimeForOldestObservationForStation_GivenStation20048_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = await dataProvider.IdentifyTimeForOldestObservationForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                stationId,
                false);

            // Assert
            time.Value.Year.Should().Be(1971);
            time.Value.Month.Should().Be(11);
            time.Value.Day.Should().Be(4);
        }

        [Fact]
        public async Task IdentifyTimeForOldestObservationForStation_GivenStation20048AndNewest_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "20048";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = await dataProvider.IdentifyTimeForOldestObservationForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                stationId,
                true);

            // Assert
            time.Value.Year.Should().Be(1991);
            time.Value.Month.Should().Be(11);
            time.Value.Day.Should().Be(12);
        }

        [Fact]
        public async Task IdentifyTimeForOldestObservationForStation_GivenStation20002AndNewest_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "20002";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = await dataProvider.IdentifyTimeForOldestObservationForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                stationId,
                true);

            // Assert
            time.Value.Year.Should().Be(DateTime.Now.Year);
            time.Value.Month.Should().Be(8);
            //time.Value.Day.Should().Be(26);
        }

        [Fact]
        public async Task IdentifyTimeForOldestObservationForStation_GivenStation20043_ReturnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);
            var stationId = "20043";

            dataProvider.Initialize(new[] {
                "sea_reg",
                "sealev_ln",
                "sealev_dvr",
                "tw"});

            // Act
            var time = await dataProvider.IdentifyTimeForOldestObservationForStation(
                _host,
                _database,
                _seaDBUser,
                _seaDBPassword,
                stationId,
                false);

            // Assert
            time.Value.Year.Should().Be(2014);
            time.Value.Month.Should().Be(3);
            time.Value.Day.Should().Be(26);
        }

    }
}