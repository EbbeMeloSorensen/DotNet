using Xunit;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.UnitTest
{
    public class StationRepositoryTest
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var unitOfWorkFactory = new UnitOfWorkFactory();
            unitOfWorkFactory.Initialize(null);

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                // Act
                var count = unitOfWork.Stations.CountAll();

                // Assert
                //count.Should().Be(1);
            }
        }
    }
}