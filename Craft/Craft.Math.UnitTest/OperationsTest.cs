using System;
using Xunit;
using FluentAssertions;

namespace Craft.Math.UnitTest
{
    public class OperationsTest
    {
        [Fact]
        public void Intersects_GivenIntersectingLines_ReturnsTrue()
        {
            // Arrange
            var line1 = new LineSegment2D(new Point2D(0, 0), new Point2D(3, 1));
            var line2 = new LineSegment2D(new Point2D(1, 1), new Point2D(2, -1));

            // Act
            var intersects = line1.Intersects(line2);

            // Assert
            intersects.Should().BeTrue();
        }

        [Fact]
        public void Intersects_GivenNonIntersectingLines_ReturnsFalse()
        {
            // Arrange
            var line1 = new LineSegment2D(new Point2D(0, 0), new Point2D(3, 1));
            var line2 = new LineSegment2D(new Point2D(2, -1), new Point2D(4, 2));

            // Act
            var intersects = line1.Intersects(line2);

            // Assert
            intersects.Should().BeFalse();
        }

        [Fact]
        public void IntersectionPointWith_GivenIntersectingLines_ReturnsCorrectResult()
        {
            // Arrange
            var line1 = new LineSegment2D(new Point2D(0, 0), new Point2D(3, 0));
            var line2 = new LineSegment2D(new Point2D(1, 1), new Point2D(2, -1));

            // Act
            var intersects = line1.IntersectionPointWith(line2, out var intersectionPoint);

            // Assert
            intersects.Should().BeTrue();
            intersectionPoint.X.Should().BeApproximately(1.5, double.Epsilon);
            intersectionPoint.Y.Should().BeApproximately(0, double.Epsilon);
        }

        [Fact]
        public void IntersectionPointWith_GivenParallelLines_ReturnsCorrectResult()
        {
            // Arrange
            var line1 = new LineSegment2D(new Point2D(0, 0), new Point2D(3, 1));
            var line2 = new LineSegment2D(new Point2D(1, 1), new Point2D(4, 2));

            // Act
            var intersects = line1.IntersectionPointWith(line2, out var intersectionPoint);

            // Assert
            intersects.Should().BeFalse();
        }

        [Fact]
        public void SideOfLine_GivenPointIsOnTheLine_ReturnsCorrectResult()
        {
            // Arrange
            var l1 = new Point2D(5, 1);
            var l2 = new Point2D(-3, 7);
            var p = new Point2D(1, 4);

            // Act
            var result = Operations.SideOfLine(p, l1, l2);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void SideOfLine_GivenPointIsOnTheLeft_ReturnsCorrectResult()
        {
            // Arrange
            var l1 = new Point2D(5, 1);
            var l2 = new Point2D(-3, 7);
            var p = new Point2D(0, 4);

            // Act
            var result = Operations.SideOfLine(p, l1, l2);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void SideOfLine_GivenPointIsOnTheRight_ReturnsCorrectResult()
        {
            // Arrange
            var l1 = new Point2D(5, 1);
            var l2 = new Point2D(-3, 7);
            var p = new Point2D(2, 4);

            // Act
            var result = Operations.SideOfLine(p, l1, l2);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void DistanceTo_GivenPointIsClosestToStartOfLineSegment_ReturnsCorrectResult()
        {
            // Arrange
            var l = new LineSegment2D(new Point2D(0, 0), new Point2D(4, 3));
            var p = new Point2D(-4, 3);

            // Act
            var distance = l.DistanceTo(p);

            // Assert
            distance.Should().BeApproximately(5, 0.00001);
        }

        [Fact]
        public void DistanceTo_GivenPointIsClosestToEndOfLineSegment_ReturnsCorrectResult()
        {
            // Arrange
            var l = new LineSegment2D(new Point2D(0, 0), new Point2D(4, 3));
            var p = new Point2D(7, 7);

            // Act
            var distance = l.DistanceTo(p);

            // Assert
            distance.Should().BeApproximately(5, 0.00001);
        }

        [Fact]
        public void DistanceTo_GivenPointIsClosestToMiddleOfLineSegment_ReturnsCorrectResult()
        {
            // Arrange
            var l = new LineSegment2D(new Point2D(0, 0), new Point2D(8, 6));
            var p = new Point2D(1, 7);

            // Act
            var distance = l.DistanceTo(p);

            // Assert
            distance.Should().BeApproximately(5, 0.00001);
        }

        [Fact]
        public void SquaredDistanceTo_GivenPointIsClosestToStartOfLineSegment_ReturnsCorrectResult()
        {
            // Arrange
            var l = new LineSegment2D(new Point2D(0, 0), new Point2D(4, 3));
            var p = new Point2D(-4, 3);

            // Act
            var squaredDistance = l.SquaredDistanceTo(p);

            // Assert
            squaredDistance.Should().BeApproximately(25, 0.00001);
        }

        [Fact]
        public void SquaredDistanceTo_GivenPointIsClosestToEndOfLineSegment_ReturnsCorrectResult()
        {
            // Arrange
            var l = new LineSegment2D(new Point2D(0, 0), new Point2D(4, 3));
            var p = new Point2D(7, 7);

            // Act
            var squaredDistance = l.SquaredDistanceTo(p);

            // Assert
            squaredDistance.Should().BeApproximately(25, 0.00001);
        }

        [Fact]
        public void SquaredDistanceTo_GivenPointIsClosestToMiddleOfLineSegment_ReturnsCorrectResult()
        {
            // Arrange
            var l = new LineSegment2D(new Point2D(0, 0), new Point2D(8, 6));
            var p = new Point2D(1, 7);

            // Act
            var squaredDistance = l.SquaredDistanceTo(p);

            // Assert
            squaredDistance.Should().BeApproximately(25, 0.00001);
        }
    }
}
