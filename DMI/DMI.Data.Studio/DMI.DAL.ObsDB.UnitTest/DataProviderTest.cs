using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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

            var credentialsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                @"Documents\credentialsForNanoq.txt");

            //var lines = File.ReadAllLines(credentialsFilePath);
            //_obsDBUser = lines[0];
            //_obsDBPassword = lines[1];
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task CheckConnection_GivenValidCredentials_ReturnsTrue()
        {
            // Arrange
            //var dataProvider = new DataProvider(null);

            //// Act
            //var connectionOK = await dataProvider.CheckConnection(_host, _database, _obsDBUser, _obsDBPassword);

            //// Assert
            //connectionOK.Should().BeTrue();
        }

    }
}
