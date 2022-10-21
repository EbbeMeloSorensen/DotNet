using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer.Repositories
{
    public class PersonRepository : Repository<Record>, IPersonRepository
    {
        public PRDbContext PrDbContext
        {
            get { return Context as PRDbContext; }
        }

        public PersonRepository(DbContext context) : base(context)
        {
        }

        public override void Update(
            Record entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<Record> people)
        {
            var listOfUpdatedPeople = people.ToList();
            var ids = listOfUpdatedPeople.Select(p => p.Id);
            var peopleFromRepository = Find(p => ids.Contains(p.Id)).ToList();

            peopleFromRepository.ForEach(pRepo =>
            {
                var updatedPerson = listOfUpdatedPeople.Single(pUpd => pUpd.Id == pRepo.Id);

                pRepo.Term = updatedPerson.Term;
                pRepo.Source = updatedPerson.Source;
                pRepo.Category = updatedPerson.Category;
                pRepo.Description = updatedPerson.Description;
                pRepo.Created = updatedPerson.Created;
            });
        }

        public Record Get(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public Record GetPersonIncludingAssociations(
            Guid id)
        {
            return PrDbContext.People
                .Include(p => p.ObjectPeople).ThenInclude(pa => pa.ObjectRecord)
                .Include(p => p.SubjectPeople).ThenInclude(pa => pa.SubjectRecord)
                .SingleOrDefault(p => p.Id == id) ?? throw new InvalidOperationException();
        }

        public IList<Record> GetPeopleIncludingAssociations(
            Expression<Func<Record, bool>> predicate)
        {
            return PrDbContext.People
                .Include(p => p.ObjectPeople).ThenInclude(pa => pa.ObjectRecord)
                .Include(p => p.SubjectPeople).ThenInclude(pa => pa.SubjectRecord)
                .Where(predicate)
                .ToList();
        }
    }
}
