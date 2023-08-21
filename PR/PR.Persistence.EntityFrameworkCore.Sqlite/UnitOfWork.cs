using PR.Persistence.EntityFrameworkCore.Repositories;
using PR.Persistence.EntityFrameworkCore.Sqlite.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContext _context;

        public IPersonRepository People { get; }
        public IPersonAssociationRepository PersonAssociations { get; }

        public UnitOfWork(PRDbContext context)
        {
            _context = context;
            People = new PersonRepositoryBase(_context);
            PersonAssociations = new PersonAssociationRepository(_context);
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