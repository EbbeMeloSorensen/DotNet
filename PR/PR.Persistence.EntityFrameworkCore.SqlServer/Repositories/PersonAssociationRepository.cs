using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.SqlServer.Repositories
{
    public class PersonAssociationRepository : Repository<PersonAssociation>, IPersonAssociationRepository
    {
        public PersonAssociationRepository(
            DbContext context) : base(context)
        {
        }

        public override void Update(
            PersonAssociation entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<PersonAssociation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
