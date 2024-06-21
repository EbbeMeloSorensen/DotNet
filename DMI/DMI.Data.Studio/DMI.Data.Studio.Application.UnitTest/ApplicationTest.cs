using FluentAssertions;

namespace DMI.Data.Studio.Application.UnitTest
{
    public class ApplicationTest
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var observationTimes = new List<DateTime>
            {
                new DateTime(2024, 7, 24, 0, 0, 0),
                new DateTime(2024, 7, 24, 1, 0, 0),
                new DateTime(2024, 7, 24, 2, 0, 0),
                new DateTime(2024, 7, 24, 3, 0, 0),

                new DateTime(2024, 7, 25, 0, 0, 0),
                new DateTime(2024, 7, 25, 1, 0, 0),
                new DateTime(2024, 7, 25, 2, 0, 0),
                new DateTime(2024, 7, 25, 3, 0, 0),

                new DateTime(2024, 7, 26, 0, 0, 0),
                new DateTime(2024, 7, 26, 1, 0, 0),
                new DateTime(2024, 7, 26, 2, 0, 0),
                new DateTime(2024, 7, 26, 3, 0, 0)
            };

            // Act
            var chunks = Application.AnalyzeTimeSeries(observationTimes);

            // Assert
            chunks
                .Select(_ => _.ObservationCount)
                .SequenceEqual(new List<int> { 4, 4, 4 })
                .Should().BeTrue();

            chunks
                .Select(_ => _.StartTime)
                .SequenceEqual(new List<DateTime>
                {
                    new DateTime(2024, 7, 24, 0, 0, 0),
                    new DateTime(2024, 7, 25, 0, 0, 0),
                    new DateTime(2024, 7, 26, 0, 0, 0)
                })
                .Should().BeTrue();

            chunks
                .Select(_ => _.EndTime)
                .SequenceEqual(new List<DateTime>
                {
                    new DateTime(2024, 7, 24, 3, 0, 0),
                    new DateTime(2024, 7, 25, 3, 0, 0),
                    new DateTime(2024, 7, 26, 3, 0, 0)
                })
                .Should().BeTrue();
        }

        [Fact]
        public void Test2()
        {
            // Arrange
            var observationTimes = new List<DateTime>
            {
                new DateTime(2024, 7, 24, 0, 0, 0),
                new DateTime(2024, 7, 24, 1, 0, 0),
                new DateTime(2024, 7, 24, 2, 0, 0),
                new DateTime(2024, 7, 24, 3, 0, 0),

                new DateTime(2024, 7, 24, 5, 0, 0),

                new DateTime(2024, 7, 25, 1, 0, 0),
                new DateTime(2024, 7, 25, 2, 0, 0),
                new DateTime(2024, 7, 25, 3, 0, 0),

                new DateTime(2024, 7, 26, 7, 0, 0)
            };

            // Act
            var chunks = Application.AnalyzeTimeSeries(observationTimes);

            // Assert
            chunks
                .Select(_ => _.ObservationCount)
                .SequenceEqual(new List<int> { 4, 1, 3, 1 })
                .Should().BeTrue();
        }

        [Fact]
        public void Test3()
        {
            // Arrange
            var observationTimes = new List<DateTime>
            {
                new DateTime(2024, 7, 23, 3, 0, 0),

                new DateTime(2024, 7, 24, 1, 0, 0),
                new DateTime(2024, 7, 24, 2, 0, 0),
                new DateTime(2024, 7, 24, 3, 0, 0),

                new DateTime(2024, 7, 24, 5, 0, 0),

                new DateTime(2024, 7, 25, 1, 0, 0),
                new DateTime(2024, 7, 25, 2, 0, 0),
                new DateTime(2024, 7, 25, 3, 0, 0),

                new DateTime(2024, 7, 26, 7, 0, 0)
            };

            // Act
            var chunks = Application.AnalyzeTimeSeries(observationTimes);

            // Assert
            chunks
                .Select(_ => _.ObservationCount)
                .SequenceEqual(new List<int> { 1, 3, 1, 3, 1 })
                .Should().BeTrue();
        }

    }
}