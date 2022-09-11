using PR.Persistence.EntityFrameworkCore.PostgreSQL.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContext _context;

        public IPersonRepository People { get; }

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
