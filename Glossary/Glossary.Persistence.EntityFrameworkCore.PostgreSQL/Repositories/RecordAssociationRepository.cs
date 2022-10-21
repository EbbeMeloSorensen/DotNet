using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.Repositories
{
    public class RecordAssociationRepository : Repository<RecordAssociation>, IRecordAssociationRepository
    {
        public RecordAssociationRepository(
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
