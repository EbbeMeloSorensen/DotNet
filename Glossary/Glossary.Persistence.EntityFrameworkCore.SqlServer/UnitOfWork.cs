using Glossary.Persistence.Repositories;
using Glossary.Persistence.EntityFrameworkCore.SqlServer.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GlossaryDbContext _context;

        public IRecordRepository Records { get; }
        public IRecordAssociationRepository RecordAssociations { get; }

        public UnitOfWork(GlossaryDbContext context)
        {
            _context = context;
            Records = new PersonRepository(_context);
            RecordAssociations = new PersonAssociationRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
