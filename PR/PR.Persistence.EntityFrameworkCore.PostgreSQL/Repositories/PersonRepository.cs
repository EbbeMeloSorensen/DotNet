using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL.Repositories
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

                pRepo.FirstName = updatedPerson.FirstName;
                pRepo.Surname = updatedPerson.Surname;
                pRepo.Nickname = updatedPerson.Nickname;
                pRepo.Address = updatedPerson.Address;
                pRepo.ZipCode = updatedPerson.ZipCode;
                pRepo.City = updatedPerson.City;
                pRepo.Birthday = updatedPerson.Birthday;
                pRepo.Category = updatedPerson.Category;
                pRepo.Description = updatedPerson.Description;
                pRepo.Created = updatedPerson.Created;
            });
        }

        public Person Get(
            Guid id)
        {
            return PrDbContext.People.Find(id);
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
