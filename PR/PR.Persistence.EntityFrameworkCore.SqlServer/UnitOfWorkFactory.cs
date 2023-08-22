namespace PR.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using var context = new PRDbContext();
            context.Database.EnsureCreated();

            if (context.People.Any()) return;

            Helpers.SeedDatabase(context);
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new PRDbContext());
        }
    }
}
