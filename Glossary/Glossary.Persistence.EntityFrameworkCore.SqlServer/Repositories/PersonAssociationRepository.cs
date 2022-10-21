using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Domain.Foreign;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer.Repositories
{
    public class PersonAssociationRepository : Repository<RecordAssociation>, IPersonAssociationRepository
    {
        public PersonAssociationRepository(
            DbContext context) : base(context)
        {
        }

        public override void Update(
            RecordAssociation entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<RecordAssociation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
