using System.Collections.Generic;
using Xunit;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.IO.UnitTest
{
    public class DataIOHandlerTest
    {
        [Fact]
        public void Test1()
        {
            var stations = new List<Station>
            {
                new Station()
                {
                    StatID = 573520,
                    Country = "Danmark",
                    IcaoId = "Bamse",
                    Source = "ing"
                },
                new Station()
                {
                    StatID = 573620,
                    Country = "Danmark",
                    IcaoId = "Kylling",
                    Source = "ing"
                },
            };

            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ExportDataToJson(stations, @"C:\Temp\Stations.json");
        }

        [Fact]
        public void Test2()
        {
            var dataIOHandler = new DataIOHandler();

            dataIOHandler.ImportDataFromJson(@"C:\Temp\Stations.json", out var stations);
        }
    }
}