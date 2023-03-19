using System.Threading.Tasks;
using System;
using DMI.Domain.FrieData;
using FluentAssertions;
using Xunit;

namespace DMI.HttpClient.GovCloud.UnitTest
{
    public class FrieDataProcessorTest
    {
        [Fact]
        public async Task LoadMeteorologicalObservations_GivenStation06041_temp_dry_January1953_ReturnsCorrectResult()
        {
            // Act
            ScalarObservation[] observations = await FrieDataProcessor.LoadScalarObservations(
                "https://dmigw.govcloud.dk/v2/metObs",
                "46cb3872-102d-426a-bc5a-5a529d47f950",
                "06041",
                "temp_dry",
                new DateTime(1953, 1, 1),
                new DateTime(1953, 2, 1),
                10000);

            // Assert
            observations.Length.Should().Be(190);
        }

        [Fact]
        public async Task LoadMeteorologicalObservations_GivenStation04041_precip_past24h_1996_ReturnsEmptyCollection()
        {
            // Act
            ScalarObservation[] observations = await FrieDataProcessor.LoadScalarObservations(
                "https://dmigw.govcloud.dk/v2/metObs",
                "46cb3872-102d-426a-bc5a-5a529d47f950",
                "06041",
                "precip_past24h",
                new DateTime(1996, 1, 1),
                new DateTime(1997, 1, 1),
                10000);

            // Assert
            observations.Length.Should().Be(0);
        }

        [Fact]
        public async Task LoadOceanographicObservations_GivenStation20043_tw_1stJanuary2019_ReturnsCorrectResult()
        {
            // Act
            //WaterTemperatureObservation[] observations = await FrieDataProcessor.LoadWaterTemperatureObservations(
            var observations = await FrieDataProcessor.LoadScalarObservations(
                "https://dmigw.govcloud.dk/v2/oceanObs",
                "6de4ae83-c739-488a-9b5c-b92f0fff122d",
                "20043",
                "tw",
                new DateTime(2019, 1, 1),
                new DateTime(2019, 1, 2),
                100000);

            // Assert
            observations.Length.Should().Be(140);
        }

        [Fact]
        public async Task LoadOceanographicObservationsV1_GivenStation20048_sealev_ln_1stHourOf1990_ReturnsCorrectResult()
        {
            // Act
            ScalarObservation[] observations = await FrieDataProcessor.LoadScalarObservations(
                "https://dmigw.govcloud.dk/v2/oceanObs",
                "6de4ae83-c739-488a-9b5c-b92f0fff122d",
                "20048",
                "sealev_ln",
                new DateTime(1990, 1, 1, 0, 0, 0),
                new DateTime(1990, 1, 1, 1, 0, 0),
                10000);

            // Assert
            observations.Length.Should().Be(4);
        }

        [Fact]
        public async Task LoadOceanographicObservationsV2_GivenStation20048_sealev_ln_1stHourOf1990_ReturnsCorrectResult()
        {
            // Act
            ScalarObservation[] observations = await FrieDataProcessor.LoadScalarObservations(
                "https://dmigw.govcloud.dk/v2/oceanObs",
                "6de4ae83-c739-488a-9b5c-b92f0fff122d",
                "20048",
                "sealev_ln",
                new DateTime(1990, 1, 1, 0, 0, 0),
                new DateTime(1990, 1, 1, 1, 0, 0),
                10000);

            // Assert
            observations.Length.Should().Be(4);
        }
    }
}
