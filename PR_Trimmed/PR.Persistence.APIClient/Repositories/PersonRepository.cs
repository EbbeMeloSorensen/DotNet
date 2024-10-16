using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
                var url = "https://api.sunrise-sunset.org/json?lat=55.661954&lng=12.49001&date=today";

                using (var response = await ApiHelper.ApiClient.GetAsync(url))
                {
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
