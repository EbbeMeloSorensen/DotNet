namespace WIGOS.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using var context = new WIGOSDbContext();

            try
            {
                context.Database.EnsureCreated();

                Seeding.SeedDatabase(context);
            }
            catch (Exception ex)
            {
                // Just swallow it for now
                // Todo: Write it in the log
                var a = ex.Message;
                throw ex;
            }
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new WIGOSDbContext());
        }
    }
}