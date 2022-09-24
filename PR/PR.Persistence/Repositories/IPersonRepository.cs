using System;
using Craft.Persistence;
using PR.Domain.Entities;

namespace PR.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        Person Get(Guid id);

        Person GetPersonIncludingAssociations(Guid id);
    }
}
