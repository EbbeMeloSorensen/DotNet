using PR.Persistence.EntityFrameworkCore.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContextBase _context;

        public IPersonRepository People { get; }
        public IPersonCommentRepository PersonComments { get; }

        public UnitOfWork(
            PRDbContextBase context)
        {
            _context = context;
            People = new PersonRepository(_context);
            PersonComments = new PersonCommentRepository(_context);
        }

        public void Clear()
        {
            //PersonAssociations.Clear();
            People.Clear();
        }

        public void Complete()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
