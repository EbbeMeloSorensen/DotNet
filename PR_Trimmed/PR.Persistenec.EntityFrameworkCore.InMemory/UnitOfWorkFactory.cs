using PR.Persistence;
using PR.Persistence.EntityFrameworkCore;

namespace PR.Persistenec.EntityFrameworkCore.InMemory
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public UnitOfWorkFactory()
        {
            using var context = new PRDbContext();
            context.Database.EnsureCreated();
            Seeding.SeedDatabase(context);
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new PRDbContext());
        }

        public void Reseed()
        {
            using var context = new PRDbContext();
            context.Database.EnsureCreated();

            using var unitOfWork = GenerateUnitOfWork();
            unitOfWork.People.Clear();
            Seeding.SeedDatabase(context);
            unitOfWork.Complete();
        }
    }
}
