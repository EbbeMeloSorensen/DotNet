using C2IEDM.Domain.Entities.ObjectItems.Organisations;
using C2IEDM.Persistence.Repositories.ObjectItems;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.ObjectItems;

public class OrganisationRepository : Repository<Organisation>, IOrganisationRepository
{
    public OrganisationRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(Organisation entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<Organisation> entities)
    {
        throw new NotImplementedException();
    }
}