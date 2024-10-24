using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Craft.Utils;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.APIClient.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private string _token;
        private DateTime? _databaseTime;

        public PersonRepository(
            DateTime? databaseTime)
        {
            _databaseTime = databaseTime;
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public async Task<Person> Get(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            await Login();

            return await Task.Run(async () =>
            {
                // The we call the API using the token - here we want all people (and we are not using pagination here)
                var url = "http://localhost:5000/api/people";

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

                    // When you know the structure of the json data
                    return JsonConvert.DeserializeObject<List<Person>>(responseBody);
                }
            });
        }

        public async Task<IEnumerable<Person>> Find(Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Person>> Find(IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Person SingleOrDefault(Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(
            Person person)
        {
            await Login();

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

        public void AddRange(IEnumerable<Person> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Person entity)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRange(IEnumerable<Person> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Person entity)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveRange(IEnumerable<Person> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<Person> entities)
        {
            throw new NotImplementedException();
        }

        private async Task Login()
        {
            var url = "http://localhost:5000/api/account/login";

            var content = new StringContent("{\"email\":\"bob@test.com\",\"password\":\"Pa$$w0rd\"}", Encoding.UTF8,
                "application/json");

            using (var response = await ApiHelper.ApiClient.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                // When you know the structure of the json data
                var result = JsonConvert.DeserializeObject<LoginResult>(responseBody);
                _token = result.token;
            }
        }
    }
}
