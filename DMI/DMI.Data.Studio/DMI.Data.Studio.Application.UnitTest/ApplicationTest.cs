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

                new DateTime(2024, 7, 24, 5, 0, 0),
                //new DateTime(2024, 7, 24, 6, 0, 0),
                //new DateTime(2024, 7, 24, 7, 0, 0),

                new DateTime(2024, 7, 25, 1, 0, 0),
                new DateTime(2024, 7, 25, 3, 0, 0),
                new DateTime(2024, 7, 25, 5, 0, 0),

                new DateTime(2024, 7, 26, 7, 0, 0)
            }; 

            // Act
            Application.AnalyzeTimeSeries(observationTimes);

            // Assert

        }
    }
}