using FluentAssertions;
using Xunit;

namespace DMI.StatDB.Persistence.Npgsql.UnitTest;

public class PositionRepositoryTest
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
            var count = unitOfWork.Positions.CountAll();

            // Assert
            count.Should().Be(2);
        }
    }
}