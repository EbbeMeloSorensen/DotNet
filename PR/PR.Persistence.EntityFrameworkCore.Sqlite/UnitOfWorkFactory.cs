namespace PR.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        // Der er vel ingen grund til at køre det som en statisk constructor.. 
        // Unit Of Work Factory laves jo under opstart i forbindelse med Dependency injection...
        //static UnitOfWorkFactory()
        //{
        //    using var context = new PRDbContext();
        //    context.Database.EnsureCreated();
        //    Seeding.SeedDatabase(context);
        //}

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
    }
}
