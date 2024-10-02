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

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(
            ContactPerson entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
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
