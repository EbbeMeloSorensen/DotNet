using DMI.StatDB.Domain.Entities;
using Xunit;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.UnitTest
{
    public class StationRepositoryTest
    {
        [Fact]
        public void CreateStation()
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

                // Assert
                //count.Should().Be(1);
            }
        }
    }
}