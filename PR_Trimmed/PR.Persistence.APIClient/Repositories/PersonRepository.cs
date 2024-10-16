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

        public IEnumerable<Person> GetAll()
        {
            var url = "https://api.sunrise-sunset.org/json?lat=55.661954&lng=12.49001&date=today";

            //await Dummy();

            throw new NotImplementedException("We want to call an API here");
        }

        private async Task Dummy()
        {
            var url = "https://api.sunrise-sunset.org/json?lat=55.661954&lng=12.49001&date=today";

            using (var response = await ApiHelper.ApiClient.GetAsync(url))
            {
            }
        }

        public IEnumerable<Person> Find(Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Person> Find(IList<Expression<Func<Person, bool>>> predicates)
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

        public void UpdateRange(IEnumerable<Person> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Person entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Person> entities)
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

        public Person Get(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
