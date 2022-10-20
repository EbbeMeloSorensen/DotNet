using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer.Repositories
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
            IEnumerable<Person> people)
        {
            var listOfUpdatedPeople = people.ToList();
            var ids = listOfUpdatedPeople.Select(p => p.Id);
            var peopleFromRepository = Find(p => ids.Contains(p.Id)).ToList();

            peopleFromRepository.ForEach(pRepo =>
            {
                var updatedPerson = listOfUpdatedPeople.Single(pUpd => pUpd.Id == pRepo.Id);

                pRepo.Term = updatedPerson.Term;
                pRepo.Address = updatedPerson.Address;
                pRepo.Category = updatedPerson.Category;
                pRepo.Description = updatedPerson.Description;
                pRepo.Created = updatedPerson.Created;
            });
        }

        public Person Get(
            Guid id)
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
