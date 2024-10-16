using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            return await Task.Run(async () =>
            {
                var url = "https://api.sunrise-sunset.org/json?lat=55.661954&lng=12.49001&date=today"; // Sol op/nde for Danshøjvej 33

                using (var response = await ApiHelper.ApiClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // When you know the structure of the json data
                    var data = JsonConvert.DeserializeObject<SunResultModel>(responseBody);

                    // Parse the JSON using JsonDocument (Suitable when you don't know the structure)
                    using (var doc = JsonDocument.Parse(responseBody))
                    {
                        var root = doc.RootElement;

                        // Navigate through JSON dynamically
                        //var id = root.GetProperty("id").GetInt32();
                        //var title = root.GetProperty("title").GetString();
                        //var completed = root.GetProperty("completed").GetBoolean();

                        //// Output the values
                        //Console.WriteLine($"ID: {id}, Title: {title}, Completed: {completed}");
                    }
                }

                return new List<Person>();
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
