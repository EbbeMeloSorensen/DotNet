using DMI.StatDB.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.UnitTest
{
    public class StationRepositoryTest
    {
        public StationRepositoryTest()
        {
            TestHelper.ClearTable("Position");
            TestHelper.ClearTable("Stations");

            TestHelper.InsertRowInTable("Stations", "(statid, country, source) VALUES (1, 'Danmark', 'WMO')");
            TestHelper.InsertRowInTable("Stations", "(statid, country, source) VALUES (2, 'Grønland', 'WMO')");

            TestHelper.InsertRowInTable("Position", "(statid, entity, starttime, lat, long) VALUES (1, 'report', '1972-03-17', 1, 2)", false);
            TestHelper.InsertRowInTable("Position", "(statid, entity, starttime, lat, long) VALUES (1, 'report', '1975-07-24', 3, 4)", false);
            TestHelper.InsertRowInTable("Position", "(statid, entity, starttime, lat, long) VALUES (2, 'report', '1980-06-13', 5, 6)", false);
        }

        [Fact]
        public void Add()
        {
            // Arrange
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                // Act
                unitOfWork.Stations.Add(new Station
                {
                    Country = "Danmark",
                    IcaoId = "bamse",
                    Source = "kylling",
                    //StatID = 12345
                });

                unitOfWork.Complete();
            }
        }

        [Fact]
        public void CountAll()
        {
            // Arrange
            var unitOfWorkFactory = new UnitOfWorkFactory();
            using var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var count = unitOfWork.Stations.CountAll();

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public void Get_GivenStationExistsInDatabase()
        {
            // Arrange
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork();
            
            // Act
            var station = unitOfWork.Stations.Get(1);

            // Assert
            station.Country.Should().Be("Danmark");
        }

        [Fact]
        public void GetWithPositions_GivenStationExistsInDatabaseAndHasPositions()
        {
            // Arrange
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var station = unitOfWork.Stations.GetWithPositions(1);

            // Assert
            station.Country.Should().Be("Danmark");
            station.Positions.Count.Should().Be(2);
        }
    }
}