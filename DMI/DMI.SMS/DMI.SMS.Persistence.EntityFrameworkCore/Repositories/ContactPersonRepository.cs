using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Repositories
{
    public class ContactPersonRepository : Repository<ContactPerson>, IContactPersonRepository
    {
        public ContactPersonRepository(
            DbContext context) : base(context)
        {
        }

        public override Task Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(
            ContactPerson entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(
            IEnumerable<ContactPerson> entities)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueObjectId()
        {
            throw new NotImplementedException();
        }

        public string GenerateUniqueGlobalId()
        {
            throw new NotImplementedException();
        }
    }
}
