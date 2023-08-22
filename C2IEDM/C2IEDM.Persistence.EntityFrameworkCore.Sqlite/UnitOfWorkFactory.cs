namespace C2IEDM.Persistence.EntityFrameworkCore.Sqlite;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    static UnitOfWorkFactory()
    {
        using var context = new C2IEDMDbContext();
        context.Database.EnsureCreated();

        //if (context.People.Any()) return;

        //Seeding.SeedDatabase(context);
    }

    public IUnitOfWork GenerateUnitOfWork()
    {
        return new UnitOfWork(new C2IEDMDbContext());
    }
}