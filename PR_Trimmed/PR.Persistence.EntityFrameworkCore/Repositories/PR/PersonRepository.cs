using Microsoft.EntityFrameworkCore;
using Craft.Logging;
using Craft.Persistence.EntityFrameworkCore;
using PR.Domain.Entities.PR;
using PR.Persistence.Repositories.PR;

namespace PR.Persistence.EntityFrameworkCore.Repositories.PR
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

        public async Task<Person> GetIncludingComments(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.People
                    .Include(p => p.Comments)
                    .SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person does not exist");
                }

                return person;
            });
        }

        public override async Task Update(
            Person person)
        {
            await Task.Run(() => { });
        }

        public override async Task UpdateRange(
            IEnumerable<Person> people)
        {
            await Task.Run(() => { });
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.People);
            await Context.SaveChangesAsync();
        }
    }
}
