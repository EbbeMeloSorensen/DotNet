using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.Repositories
{
    public class RecordAssociationRepository : Repository<RecordAssociation>, IRecordAssociationRepository
    {
        public GlossaryDbContext PrDbContext
        {
            get { return Context as GlossaryDbContext; }
        }

        public RecordAssociationRepository(
            DbContext context) : base(context)
        {
        }

        public RecordAssociation Get(
            Guid id)
        {
            return PrDbContext.RecordAssociations.Find(id);
        }

        public override void Update(
            RecordAssociation recordAssociation)
        {
            var objFromRepository = Get(recordAssociation.Id);

            objFromRepository.SubjectRecordId = recordAssociation.SubjectRecordId;
            objFromRepository.ObjectRecordId = recordAssociation.ObjectRecordId;
            objFromRepository.Description = recordAssociation.Description;
        }

        public override void UpdateRange(
            IEnumerable<RecordAssociation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
