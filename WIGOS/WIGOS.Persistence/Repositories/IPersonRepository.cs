using WIGOS.Domain.Entities;
using Craft.Persistence;

namespace WIGOS.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        Person Get(
            Guid id);
    }
}
