using System;
using System.Collections.Generic;
using Xunit;
using DMI.StatDB.Domain.Entities;
using FluentAssertions;

namespace DMI.StatDB.IO.UnitTest
{
    public class DataIOHandlerTest
    {
        [Fact]
        public void Test1()
        {
            var station1 = new Station()
            {
                StatID = 573520,
                Country = "Danmark",
                IcaoId = "Bamse",
                Source = "ing"
            };

            var station2 = new Station()
            {
                StatID = 573620,
                Country = "Danmark",
                IcaoId = "Kylling",
                Source = "ing"
            };

            var stations = new List<Station>
            {
                station1,
                station2
            };

            var positions = new List<Position>
            {
                new Position
                {
                    Station = station1,
                    StatID = 573520,
                    Height = 15,
                    Lat = 5.5,
                    Long = 10.2,
                    StartTime = new DateTime(1972, 3, 17)
                },
                new Position
                {
                    Station = station1,
                    StatID = 573520,
                    Height = 15,
                    Lat = 5.5,
                    Long = 10.2,
                    StartTime = new DateTime(1975, 7, 24)
                },
                new Position
                {
                    Station = station2,
                    StatID = 573620,
                    Height = 15,
                    Lat = 5.5,
                    Long = 10.2,
                    StartTime = new DateTime(1980, 6, 13)
                }
            };

            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ExportDataToJson(stations, positions, "Temp.json");
        }

        [Fact]
        public void Test2()
        {
            var dataIOHandler = new DataIOHandler();

            dataIOHandler.ImportDataFromJson(
                @"Data/StatDBData.json",
                out var stations,
                out var positions);

            stations.Count.Should().Be(2);
            positions.Count.Should().Be(3);
        }
    }
}