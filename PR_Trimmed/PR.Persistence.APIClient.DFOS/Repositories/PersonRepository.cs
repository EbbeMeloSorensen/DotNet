using System.Globalization;
using Craft.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;
using PR.Domain.DFOS;
using PR.Persistence.Repositories;

namespace PR.Persistence.APIClient.DFOS.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private static DateTime _maxDate;

        static PersonRepository()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private string _baseURL;
        private string _token; // Den her burde kunne undværes for DFOS APIet, som jo indtil videre kører uden authentification
        private DateTime? _historicalTime;
        private DateTime? _databaseTime;

        public PersonRepository(
            ILogger logger,
            string baseURL,
            DateTime? historicalTime,
            DateTime? databaseTime)
        {
            Logger = logger;
            _baseURL = baseURL;
            _historicalTime = historicalTime;
            _databaseTime = databaseTime;
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public ILogger Logger { get; }

        public async Task<Person> Get(
            Guid id)
        {
            // We call the API using the token - here we want all people (and we are not using pagination here)
            var url = $"http://localhost:5000/api/people/{id}";

            if (_databaseTime.HasValue)
            {
                //url = "http://localhost:5000/api/people?DatabaseTime=2002-01-01T00:00:00Z";
                url += $"?DatabaseTime={_databaseTime.Value.AsRFC3339(false)}";
            }

            ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            using var response = await ApiHelper.ApiClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var person = JsonConvert.DeserializeObject<Person>(responseBody);
            return person;
        }

        public Task<IEnumerable<Person>> GetAllVariants(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema()
        {
            string responseBody;

            if (false)
            {
                var fileName = @".\Data\mock_response.json";
                responseBody = File.ReadAllText(fileName, Encoding.UTF8);
            }
            else
            {
                var url = $"{_baseURL}/collections/observing_facility/items?datetime=..%2F..";

                using var response = await ApiHelper.ApiClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

                ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);
            }

            var dfosResult = JsonConvert.DeserializeObject<DFOSResultModel>(responseBody);

            var timeStamps = new List<DateTime>();

            try
            {
                foreach (var feature in dfosResult.Features)
                {
                    var timeStampsForFeature = new List<DateTime>();

                    foreach (var detail in feature.Properties.Details)
                    {
                        var startTimeAsText = detail.Key.Split(",")[0];
                        var endTimeAsText = detail.Key.Split(",")[1];

                        var pattern = @"(\s+|\(|\)|\[|\])";
                        startTimeAsText = Regex.Replace(startTimeAsText, pattern, "");
                        endTimeAsText = Regex.Replace(endTimeAsText, pattern, "");

                        string[] formats =
                        {
                            "yyyy-MM-ddTHH:mm:ssZ",
                            "yyyy-MM-ddTHH:mm:ss.fffZ",
                            "yyyy-MM-ddTHH:mm:ss.ffZ"
                        };

                        var startTime = DateTime.ParseExact(startTimeAsText, formats,
                            CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                        var endTime = string.IsNullOrEmpty(endTimeAsText)
                            ? DateTime.MaxValue
                            : DateTime.ParseExact(endTimeAsText, formats,
                                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                        timeStampsForFeature.Add(startTime);
                        timeStampsForFeature.Add(endTime);

                        timeStampsForFeature = timeStampsForFeature.Distinct().OrderBy(_ => _).ToList();

                        if (timeStampsForFeature.Last().Year == 9999)
                        {
                            timeStampsForFeature = timeStampsForFeature.SkipLast(1).ToList();
                        }

                        timeStamps.Add(timeStampsForFeature.First());
                        timeStamps.Add(timeStampsForFeature.Last());
                    }
                }
            }
            catch (Exception e)
            {
                var a = 0;
            }

            timeStamps = timeStamps.Distinct().OrderBy(_ => _).ToList();


            return timeStamps;
        }

        public async Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes()
        {
            return await Task.Run(() =>
            {
                // Just return an empty list for now. Later add it to the API
                return new List<DateTime>();
            });
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var observingFacilities = new List<ObservingFacility>();

            var startOfURL = $"{_baseURL}/collections/observing_facility/items";

            var people = new List<Person>();

            var useHRefFromResponse = false;
            int? pageSize = 5;
            var morePages = true;
            Guid? offset = null;
            string href = null;
            string url = null;

            do
            {
                if (useHRefFromResponse && !string.IsNullOrEmpty(href))
                {
                    url = href;
                }
                else
                {
                    var arguments = new List<string>();

                    if (_historicalTime.HasValue)
                    {
                        arguments.Add($"datetime={_historicalTime.Value.AsRFC3339(false)}");
                    }

                    if (_databaseTime.HasValue)
                    {
                        arguments.Add($"revision-time={_databaseTime.Value.AsRFC3339(false)}");
                    }

                    if (pageSize.HasValue)
                    {
                        arguments.Add($"limit={pageSize.Value}");
                    }

                    if (offset.HasValue)
                    {
                        arguments.Add($"offset={offset.Value.ToString()}");
                    }

                    url = startOfURL;

                    if (arguments.Any())
                    {
                        url += "?";
                        url += arguments.Aggregate((c, n) => $"{c}&{n}");
                    }
                }

                Logger?.WriteLine(LogMessageCategory.Information, url);

                using var response = await ApiHelper.ApiClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var dfosResult = JsonConvert.DeserializeObject<DFOSResultModel>(responseBody);

                foreach (var feature in dfosResult.Features)
                {
                    string nameBefore = null;
                    double latitudeBefore = double.NaN;
                    double longitudeBefore = double.NaN;

                    foreach (var kvp in feature.Properties.Details)
                    {
                        var pattern = @"(\s+|\(|\)|\[|\])";
                        var startTimeAsText = kvp.Key.Split(",")[0];
                        var endTimeAsText = kvp.Key.Split(",")[1];
                        startTimeAsText = Regex.Replace(startTimeAsText, pattern, "");
                        endTimeAsText = Regex.Replace(endTimeAsText, pattern, "");

                        string[] formats =
                        {
                            "yyyy-MM-ddTHH:mm:ssZ",
                            "yyyy-MM-ddTHH:mm:ss.fffZ",
                            "yyyy-MM-ddTHH:mm:ss.ffZ"
                        };

                        var startTime = DateTime.ParseExact(startTimeAsText, formats,
                            CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                        var endTime = string.IsNullOrEmpty(endTimeAsText)
                            ? new DateTime(9999, 12, 31, 23, 59, 59)
                            : DateTime.ParseExact(endTimeAsText, formats,
                                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                        var name = kvp.Value.FacilityName;
                        var latitude = kvp.Value.GeoLocation.Coordinates[0];
                        var longitude = kvp.Value.GeoLocation.Coordinates[1];

                        if (double.IsNaN(latitudeBefore) ||
                            double.IsNaN(longitudeBefore) ||
                            latitude != latitudeBefore ||
                            longitude != longitudeBefore ||
                            name != nameBefore)
                        {
                            people.Add(new Person
                            {
                                ID = feature.Id,
                                Created = new DateTime(2000, 1, 1),
                                Superseded = _maxDate,
                                Start = startTime,
                                End = endTime,
                                FirstName = name,
                                Latitude = latitude,
                                Longitude = longitude
                            });
                        }

                        latitudeBefore = latitude;
                        longitudeBefore = longitude;
                        nameBefore = name;
                    }
                }

                var nextLink = dfosResult.Links.SingleOrDefault(_ => _.Rel == "next");

                if (nextLink != null)
                {
                    href = nextLink.HRef; // Ifølge Lars behøver vi ikke bruge denne

                    //offset = dfosResult.Features.Max(_ => _.Id); // Dette er Lars' anbefalede metode
                    offset = dfosResult.Features.Last().Id; // Dette svarer til at bruge HRef
                }
                else
                {
                    morePages = false;
                }
            }
            while (morePages);

            return people;
        }

        public async Task<IEnumerable<Person>> Find(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Person>> Find(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Person SingleOrDefault(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(
            Person person)
        {
            var url = "http://localhost:5000/api/people";

            ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            var body = $"{{\"id\":\"{Guid.NewGuid()}\",\"firstName\":\"{person.FirstName}\"}}";
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            using (var response = await ApiHelper.ApiClient.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task AddRange(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            Person person)
        {
            await Task.Run(async () =>
            {
                var url = $"http://localhost:5000/api/people/{person.ID}";

                ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                var body = $"{{\"firstName\":\"{person.FirstName}\"}}";
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                using var response = await ApiHelper.ApiClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
            });
        }

        public async Task UpdateRange(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(
            Person person)
        {
            await Task.Run(async () =>
            {
                var url = $"http://localhost:5000/api/people/{person.ID}";

                ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                using var response = await ApiHelper.ApiClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
            });
        }

        public async Task RemoveRange(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }

        public async Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }
    }
}
