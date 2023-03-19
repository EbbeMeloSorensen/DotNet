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
            var url = GenerateURL(govCloudBasisUrl, govCloudAPIKey, stationId, parameterId, from, to, limit);

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
    }
}
