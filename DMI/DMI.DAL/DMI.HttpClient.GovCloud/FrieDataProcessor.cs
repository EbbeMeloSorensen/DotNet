using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DMI.Utils;
using DMI.Domain.FrieData;
using DMI.Domain.FrieData.OGC;

namespace DMI.HttpClient.GovCloud
{
    public class FrieDataProcessor
    {
        public static async Task<bool> CheckConnection(
            string govCloudBasisUrl,
            string apiKey)
        {
            var url = govCloudBasisUrl;

            if (govCloudBasisUrl.Contains("v1"))
            {
                url += $"/observation";
            }
            else
            {
                url += $"/collections/observation/items";
            }

            if (apiKey != null && apiKey != "")
            {
                url += $"?api-key={apiKey}";
            }

            using (var response = await ApiHelper.ApiClient.GetAsync(url))
            {
                return response.IsSuccessStatusCode;
            }
        }

        public static string GenerateURL(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            DateTime dateTimeFrom,
            DateTime dateTimeTo,
            int? limit)
        {
            if (govCloudBasisUrl.Contains("v1"))
            {
                var timeStampFrom = dateTimeFrom.AsEpochInMicroSeconds();
                var timeStampTo = dateTimeTo.AsEpochInMicroSeconds();

                return GenerateURLForV1(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, timeStampFrom, timeStampTo, limit);
            }
            else
            {
                return GenerateURLForV2(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, dateTimeFrom, dateTimeTo, limit);
            }
        }

        public static string GenerateURLForV1(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            long timeStampFrom,
            long timeStampTo,
            int? limit)
        {
            var url = govCloudBasisUrl + $"/observation?";

            if (govCloudAPIKey != null && govCloudAPIKey != "")
            {
                url += $"api-key={govCloudAPIKey}&";
            }

            url += $"stationId={stationId}&parameterId={parameterId}&from={timeStampFrom}&to={timeStampTo}";

            if (limit.HasValue)
            {
                url += $"&limit={limit.Value}";
            }

            return url;
        }

        public static string GenerateURLForV2(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            DateTime from,
            DateTime to,
            int? limit)
        {
            var url = govCloudBasisUrl + $"/collections/observation/items?";

            if (govCloudAPIKey != null && govCloudAPIKey != "")
            {
                url += $"api-key={govCloudAPIKey}&";
            }

            // Vi korrigerer, fordi standarden er at operere med et interval,
            // der inkluderer begge ender af intervallet, men vi har brug for at
            // ekskludere slutningen af intervallet
            var correctedTo = to - new TimeSpan(0, 0, 0, 0, 1);

            var fromAsZulu = from.AsRFC3339(false);
            var toAsZulu = correctedTo.AsRFC3339(true);

            url += $"stationId={stationId}&parameterId={parameterId}&datetime={fromAsZulu}/{toAsZulu}";

            if (limit.HasValue)
            {
                url += $"&limit={limit.Value}";
            }

            return url;
        }

        public static async Task<ScalarObservation[]> LoadScalarObservations(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            DateTime from,
            DateTime to,
            int? limit)
        {
            if (govCloudBasisUrl.Contains("v1"))
            {
                var timeStampFrom = from.AsEpochInMicroSeconds();
                var timeStampTo = to.AsEpochInMicroSeconds();

                return await LoadScalarObservationsForV1(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, timeStampFrom, timeStampTo, limit);
            }
            else
            {
                return await LoadScalarObservationsForV2(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, from, to, limit);
            }
        }

        public static async Task<ScalarObservation[]> LoadScalarObservationsForV1(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            long timeStampFrom,
            long timeStampTo,
            int? limit)
        {
            var url = GenerateURLForV1(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, timeStampFrom, timeStampTo, limit);

            using (var response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var observations = await response.Content.ReadAsAsync<ScalarObservation[]>();

                        if (observations == null)
                        {
                            return new ScalarObservation[0];
                        }

                        return observations;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<ScalarObservation[]> LoadScalarObservationsForV2(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            DateTime from,
            DateTime to,
            int? limit)
        {
            var url = GenerateURLForV2(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, from, to, limit);

            using (var response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var observations = await response.Content.ReadAsAsync<ObservationResponse>();

                        if (observations == null)
                        {
                            return new ScalarObservation[0];
                        }

                        return observations.features
                            .Select(f => new ScalarObservation
                            {
                                value = f.properties.value,
                                timeCreated = f.properties.created.ConvertFromRFC3339StringToEpoch().Value,
                                timeObserved = f.properties.observed.ConvertFromRFC3339StringToEpoch().Value
                            })
                            .ToArray();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<WaterTemperatureObservation[]> LoadWaterTemperatureObservations(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            DateTime dateTimeFrom,
            DateTime dateTimeTo,
            int? limit)
        {
            var timeStampFrom = dateTimeFrom.AsEpochInMicroSeconds();
            var timeStampTo = dateTimeTo.AsEpochInMicroSeconds();

            return await LoadWaterTemperatureObservations(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, timeStampFrom, timeStampTo, limit);
        }

        public static async Task<WaterTemperatureObservation[]> LoadWaterTemperatureObservations(
            string govCloudBasisUrl,
            string govCloudAPIKey,
            string stationId,
            string parameterId,
            long timeStampFrom,
            long timeStampTo,
            int? limit)
        {
            var url = GenerateURLForV1(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, timeStampFrom, timeStampTo, limit);

            using (var response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var observations = await response.Content.ReadAsAsync<WaterTemperatureObservation[]>();

                        if (observations == null)
                        {
                            return new WaterTemperatureObservation[0];
                        }

                        return observations;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
