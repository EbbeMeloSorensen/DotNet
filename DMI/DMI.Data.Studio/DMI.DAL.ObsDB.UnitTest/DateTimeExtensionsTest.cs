using System;
using Xunit;
using FluentAssertions;
using DMI.Utils;

namespace DMI.DAL.ObsDB.UnitTest
{
    public class DateTimeExtensionsTest
    {
        [Fact]
        public void AsString_ReturnsCorrectResult()
        {
            // Arrange
            var dateTime = new DateTime(2011, 4, 3, 15, 7, 42);

            // Act
            var asString = dateTime.AsDateTimeString(false);

            // Assert
            asString.Should().Be("2011-04-03 15:07:42");
        }

        [Fact]
        public void AsShortString_ReturnsCorrectResult()
        {
            // Arrange
            var dateTime = new DateTime(2011, 4, 3);

            // Act
            var asString = dateTime.AsDateString();

            // Assert
            asString.Should().Be("2011-04-03");
        }
    }
}