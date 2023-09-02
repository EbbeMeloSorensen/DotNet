using C2IEDM.Domain.Entities;
using Craft.Persistence;

namespace C2IEDM.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        Person Get(
            Guid id);
    }
}
