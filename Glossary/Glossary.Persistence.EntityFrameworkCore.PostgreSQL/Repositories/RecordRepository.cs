using System.Linq.Expressions;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.Repositories
{
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public GlossaryDbContext PrDbContext
        {
            get { return Context as GlossaryDbContext; }
        }

        public RecordRepository(DbContext context) : base(context)
        {
        }

        public override void Update(
            Record entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(
            IEnumerable<Record> records)
        {
            var listOfUpdatedRecords = records.ToList();
            var ids = listOfUpdatedRecords.Select(p => p.Id);
            var recordsFromRepository = Find(p => ids.Contains(p.Id)).ToList();

            recordsFromRepository.ForEach(pRepo =>
            {
                var updatedRecord = listOfUpdatedRecords.Single(pUpd => pUpd.Id == pRepo.Id);

                pRepo.Term = updatedRecord.Term;
                pRepo.Source = updatedRecord.Source;
                pRepo.Category = updatedRecord.Category;
                pRepo.Description = updatedRecord.Description;
                pRepo.Created = updatedRecord.Created;
            });
        }

        public Record Get(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public Record GetRecordIncludingAssociations(
            Guid id)
        {
            return PrDbContext.Records
                .Include(p => p.ObjectRecords).ThenInclude(pa => pa.ObjectRecord)
                .Include(p => p.SubjectRecords).ThenInclude(pa => pa.SubjectRecord)
                .SingleOrDefault(p => p.Id == id) ?? throw new InvalidOperationException();
        }

        public IList<Record> GetRecordsIncludingAssociations(
            Expression<Func<Record, bool>> predicate)
        {
            return PrDbContext.Records
                .Include(p => p.ObjectRecords).ThenInclude(pa => pa.ObjectRecord)
                .Include(p => p.SubjectRecords).ThenInclude(pa => pa.SubjectRecord)
                .Where(predicate)
                .ToList();
        }
    }
}
