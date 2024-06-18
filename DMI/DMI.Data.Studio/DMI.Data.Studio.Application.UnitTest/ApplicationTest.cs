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
            }; 

            // Act
            Application.AnalyzeTimeSeries(observationTimes);

            // Assert

        }
    }
}