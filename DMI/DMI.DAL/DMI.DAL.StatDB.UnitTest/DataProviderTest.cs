using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DMI.DAL.StatDB.UnitTest
{
    public class DataProviderTest
    {
        //private const string host = "nanoq.dmi.dk"; // nanoq4, den operationelle database for observationer
        private const string _host = "nanoq-ro.dmi.dk"; // nanoq4, dvs mirror databasen for nanoq3
        private const string _statDBdatabase = "statdb";
        private const string _statDBParamDatabase = "statdb_parameter";
        private const string _statDBUser = "ebs";
        private const string _statDBPassword = "secret";

        [Fact]
        public async Task CheckConnection_GivenValidCredentials_ReturnsTrue()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(
                _host,
                _statDBdatabase,
                _statDBUser,
                _statDBPassword);

            // Assert
            connectionOK.Should().BeTrue();
        }

        [Fact]
        public async Task CheckConnection_GivenInvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var connectionOK = await dataProvider.CheckConnection(
                _host,
                _statDBdatabase,
                _statDBUser,
                "wrongPassword");

            // Assert
            connectionOK.Should().BeFalse();
        }

        [Fact]
        public async Task RetrieveAllStationIds()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var stationIds = await dataProvider.RetrieveDataFromStationInformationTable(
                _host,
                _statDBdatabase,
                _statDBUser,
                _statDBPassword);

            // Assert
            stationIds.Count.Should().Be(15856);
        }

        [Fact]
        public async Task RetrieveStationParameterMapFromStatParameterTable_returnsCorrectResult()
        {
            // Arrange
            var dataProvider = new DataProvider(null);

            // Act
            var stationParameterMap = await dataProvider.RetrieveStationParameterMapFromStatParameterTable(
                _host,
                _statDBParamDatabase,
                _statDBUser,
                _statDBPassword);

            // Assert
            stationParameterMap.Count.Should().Be(385);
            stationParameterMap.ContainsKey(587920).Should().BeTrue();
            stationParameterMap[587920].Count.Should().Be(19);
            stationParameterMap[587920].Contains("precip_dur_past10min").Should().BeTrue();
        }
    }
}