using Craft.Math;
using FluentAssertions;
using Simulator.Domain.Props;
using Xunit;

namespace Simulator.Domain.UnitTest.Props
{
    public class PropRectangleTest
    {
        [Fact]
        public void DistanceToPointTest()
        {
            // Arrange
            var propRectangle = new PropRectangle(1, 2, 1, new Vector2D(1, 1));
            var point = new Vector2D(2, 2);

            // Act
            var distance = propRectangle.DistanceToPoint(point);

            // Assert
            distance.Should().BeApproximately(.5, 0000.1);
        }
    }
}