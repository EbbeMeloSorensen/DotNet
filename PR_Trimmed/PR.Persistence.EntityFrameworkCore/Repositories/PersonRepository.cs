﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<Person> Get(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.People.SingleOrDefault(p => p.Id == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person does not exist");
                }

                return person;
            });
        }

        public override void Update(
            Person person)
        {
            throw new NotImplementedException();
        }

        public override async Task UpdateRange(
            IEnumerable<Person> people)
        {
            await Task.Run(async () =>
            {
                var updatedPeople = people.ToList();
                var ids = updatedPeople.Select(p => p.Id);
                var peopleFromRepository = (await Find(p => ids.Contains(p.Id))).ToList();

                peopleFromRepository.ForEach(pRepo =>
                {
                    var updatedPerson = updatedPeople.Single(pUpd => pUpd.Id == pRepo.Id);

                    pRepo.FirstName = updatedPerson.FirstName;
                    pRepo.Surname = updatedPerson.Surname;
                    pRepo.Created = updatedPerson.Created;
                });
            });
        }

        public override void Clear()
        {
            Context.RemoveRange(PrDbContext.People);
            Context.SaveChanges();
        }
    }
}
