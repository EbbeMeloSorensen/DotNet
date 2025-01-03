﻿using Craft.Logging;
using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;
using PR.Domain;

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
            await Task.Run(() => {});
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
