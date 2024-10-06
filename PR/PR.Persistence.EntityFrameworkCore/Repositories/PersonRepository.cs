using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public PersonRepository(
            DbContext context) : base(context)
        {
        }

        public override void Add(
            Person person)
        {
            person.Created = DateTime.Now;
            base.Add(person);
        }

        public override void Update(
            Person person)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<Person> people)
        {
            var updatedPeople = people.ToList();
            var ids = updatedPeople.Select(p => p.Id);
            var peopleFromRepository = Find(p => ids.Contains(p.Id)).ToList();

            peopleFromRepository.ForEach(pRepo =>
            {
                var updatedPerson = updatedPeople.Single(pUpd => pUpd.Id == pRepo.Id);

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
            return PrDbContext.People.Single(_ => _.Id == id);
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

        public Person GetObject(
            Guid objectId, 
            DateTime? databaseTime)
        {
            if (databaseTime.HasValue)
            {
                return Find(p => p.ObjectId == objectId &&
                                 p.Created <= databaseTime &&
                                 p.Superseded > databaseTime).Single();
            }
            
            return Find(p => p.ObjectId == objectId && p.Superseded.Year == 9999).Single();
        }

        public override void Clear()
        {
            Context.RemoveRange(PrDbContext.People);
            Context.SaveChanges();
        }
    }
}
