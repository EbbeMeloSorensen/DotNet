using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.APIClient.Repositories
{
    public class PersonRepository : IPersonRepository
    {
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
            // First we log in
            var url = "http://localhost:5000/api/account/login";
            string token;

            var content = new StringContent("{\"email\":\"bob@test.com\",\"password\":\"Pa$$w0rd\"}", Encoding.UTF8,
                "application/json");
            using (var response = await ApiHelper.ApiClient.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                // When you know the structure of the json data
                var result = JsonConvert.DeserializeObject<LoginResult>(responseBody);
                token = result.token;
            }

            return await Task.Run(async () =>
            {
                // The we call the API using the token - here we want all people (and we are not using pagination here)
                var url = "http://localhost:5000/api/people";

                ApiHelper.ApiClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

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

        public void Add(Person entity)
        {
            throw new NotImplementedException();
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
    }
}
