namespace WIGOS.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using var context = new WIGOSDbContext();
            context.Database.EnsureCreated();

            if (context.Locations.Any()) return;

            Seeding.SeedDatabase(context);
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new WIGOSDbContext());
        }
    }
}