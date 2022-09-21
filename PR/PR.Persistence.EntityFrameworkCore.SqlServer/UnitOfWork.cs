using PR.Persistence.EntityFrameworkCore.SqlServer.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContext _context;

        public IPersonRepository People { get; }
        public IPersonAssociationRepository PersonAssociations { get; }

        public UnitOfWork(PRDbContext context)
        {
            _context = context;
            People = new PersonRepository(_context);
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
