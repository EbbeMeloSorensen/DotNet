using Newtonsoft.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace PR.Persistence.APIClient.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public async void CallSimpleWebAPIAndDeserializeResult()
        {
            var url = "https://api.sunrise-sunset.org/json?lat=55.661954&lng=12.49001&date=today"; // Sun up/down for Danshøjvej 33

            using var response = await ApiHelper.ApiClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // When you know the structure of the json data
            var data = JsonConvert.DeserializeObject<SunResultModel>(responseBody);
            data.Results.Sunrise.Date.Should().Be(DateTime.Today);
            data.Results.Sunset.Date.Should().Be(DateTime.Today);
        }

        [Fact]
        public async void CallSimpleWebAPIAndObtainResultWithoutKnowingStructure()
        {
            var url = "https://api.sunrise-sunset.org/json?lat=55.661954&lng=12.49001&date=today"; // Sun up/down for Danshøjvej 33

            using (var response = await ApiHelper.ApiClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON using JsonDocument (Suitable when you don't know the structure)
                using (var doc = JsonDocument.Parse(responseBody))
                {
                    var root = doc.RootElement;

                    // Navigate through JSON dynamically
                    var results = root.GetProperty("results");
                    var sunrise = results.GetProperty("sunrise");
                    var sunset = results.GetProperty("sunset");
                }
            }
        }

        [Fact]
        public async void GetAllPeople()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var people = await unitOfWork.People.GetAll();
                people.Count().Should().BeGreaterThan(0);
            }
        }
    }
}