using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(DbContext context) : base(context)
        {
        }

        public override void Update(
            Person entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<Person> entities)
        {
            throw new NotImplementedException();
        }

        public Person Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Person GetPersonIncludingAssociations(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public IList<Person> GetPeopleIncludingAssociations(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
