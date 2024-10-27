using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Persistence.Repositories.ObjectItems;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.ObjectItems
{
    public class OrganisationRepository : Repository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(Organisation entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Organisation> entities)
        {
            throw new NotImplementedException();
        }
    }
}