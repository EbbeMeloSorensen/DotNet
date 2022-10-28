using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PRDbContext PrDbContext
        {
            get { return Context as PRDbContext; }
        }

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
            return PrDbContext.People
                .Include(p => p.ObjectPeople).ThenInclude(pa => pa.ObjectPerson)
                .Include(p => p.SubjectPeople).ThenInclude(pa => pa.SubjectPerson)
                .SingleOrDefault(p => p.Id == id) ?? throw new InvalidOperationException();
        }

        public IList<Person> GetPeopleIncludingAssociations(
            Expression<Func<Person, bool>> predicate)
        {
            return PrDbContext.People
                .Include(p => p.ObjectPeople).ThenInclude(pa => pa.ObjectPerson)
                .Include(p => p.SubjectPeople).ThenInclude(pa => pa.SubjectPerson)
                .Where(predicate)
                .ToList();
        }
    }
}
