using Craft.Logging;
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

        public ILogger Logger { get; }

        public async Task<Person> Get(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.People.SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person does not exist");
                }

                return person;
            });
        }

        public Task<IEnumerable<Person>> GetAllVariants(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes()
        {
            throw new NotImplementedException();
        }

        public override async Task Update(
            Person person)
        {
            await Task.Run(async () =>
            {
                // Jeg er sgu ikke sikker på, at dette er nødvendigt...
                // i hvert fald ikke, når det objekt, der sendes ned, er det trackede objekt - så er det nemlig det samme,
                // som man får ved dette kald...
                //var personFromRepository = await Get(person.ID);
                //personFromRepository.FirstName = person.FirstName;
                //personFromRepository.Surname = person.Surname;
            });
        }

        public override async Task UpdateRange(
            IEnumerable<Person> people)
        {
            // Hvad er ideen her?
            // ..du unlader at gøre dig nogen antagelser om at de objekter, der sendes ned, er trackede.
            // derfor trækker du dem fra repoet, og så ændrer du dem

            await Task.Run(async () =>
            {
                var updatedPeople = people.ToList();
                var ids = updatedPeople.Select(p => p.ID);
                var peopleFromRepository = (await Find(p => ids.Contains(p.ID))).ToList();

                peopleFromRepository.ForEach(pRepo =>
                {
                    var updatedPerson = updatedPeople.Single(pUpd => pUpd.ID == pRepo.ID);

                    pRepo.FirstName = updatedPerson.FirstName;
                    pRepo.Surname = updatedPerson.Surname;
                    pRepo.Created = updatedPerson.Created;
                });
            });
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.People);
            await Context.SaveChangesAsync();
        }
    }
}
