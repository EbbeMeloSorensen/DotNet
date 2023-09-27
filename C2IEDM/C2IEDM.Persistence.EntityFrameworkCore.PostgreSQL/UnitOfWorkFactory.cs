namespace C2IEDM.Persistence.EntityFrameworkCore.PostgreSQL;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    static UnitOfWorkFactory()
    {
        using var context = new C2IEDMDbContext();

        try
        {
            if (true)
            {
                context.Database.EnsureCreated();

                if (context.Locations.Any()) return;

                Seeding.SeedDatabase(context);
            }
        }
        catch(Npgsql.NpgsqlException ex)
        {
            // Just swallow it for now
            // Todo: Write it in the log
            var a = ex.Message;
        }
    }

    public IUnitOfWork GenerateUnitOfWork()
    {
        return new UnitOfWork(new C2IEDMDbContext());
    }
}