﻿using System.Globalization;
using Craft.Utils;
using Newtonsoft.Json;
using PR.Domain.Entities;
using PR.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PR.Persistence.APIClient.DFOS.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private static DateTime _maxDate;

        static PersonRepository()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private string _token;
        private DateTime? _historicalTime;
        private DateTime? _databaseTime;

        public PersonRepository(
            DateTime? historicalTime,
            DateTime? databaseTime)
        {
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

        public async Task<Person> Get(
            Guid id)
        {
            return await Task.Run(async () =>
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

                using (var response = await ApiHelper.ApiClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var person = JsonConvert.DeserializeObject<Person>(responseBody);
                    return person;
                }
            });
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var observingFacilities = new List<ObservingFacility>();

            var environment = "dev";
            var url = $"http://dfos-api-{environment}.dmi.dk/collections/observing_facility/items";

            var arguments = new List<string>();

            if (_historicalTime.HasValue)
            {
                arguments.Add($"HistoricalTime={_historicalTime.Value.AsRFC3339(false)}");
            }

            if (_databaseTime.HasValue)
            {
                arguments.Add($"DatabaseTime={_databaseTime.Value.AsRFC3339(false)}");
            }

            if (arguments.Any() && false)
            {
                url += "?";
                url += arguments.Aggregate((c, n) => $"{c}&{n}");
            }

            using var response = await ApiHelper.ApiClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            var dfosResult = JsonConvert.DeserializeObject<DFOSResultModel>(responseBody);
            
            // Vi skal mappe det til personer, sådan at vi trawler alle features igennem,
            // og for hver af dem trawler vi dens details igennem

            var people = new List<Person>();

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

                    var startTime = DateTime.ParseExact(startTimeAsText, "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                    var endTime = string.IsNullOrEmpty(endTimeAsText)
                        ? new DateTime(9999, 12, 31, 23, 59, 59)
                        : DateTime.ParseExact(endTimeAsText, "yyyy-MM-ddTHH:mm:ssZ",
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

        public void Clear()
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