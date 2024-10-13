using PR.Persistence.EntityFrameworkCore.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContextBase _context;

        public IPersonRepository People { get; }

        public UnitOfWork(
            PRDbContextBase context)
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
