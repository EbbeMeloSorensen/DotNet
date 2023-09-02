using C2IEDM.Domain.Entities;
using C2IEDM.Persistence.Repositories;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        private C2IEDMDbContextBase PrDbContext => Context as C2IEDMDbContextBase;

        public PersonRepository(
            DbContext context) : base(context)
        {
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

        public Person Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
