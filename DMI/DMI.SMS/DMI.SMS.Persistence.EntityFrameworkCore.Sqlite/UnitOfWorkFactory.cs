using Craft.Logging;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : UnitOfWorkFactoryBase
    {
        static UnitOfWorkFactory()
        {
            using var context = new SMSDbContext();
            context.Database.EnsureCreated();

            if (context.StationInformations.Any()) return;

            Seeding.SeedDatabase(context);
        }

        public override void Initialize(ILogger logger)
        {
        }

        public override async Task<bool> CheckRepositoryConnection()
        {
            return await Task.Run(() =>
            {
                using var context = new SMSDbContext();
                return context.Database.CanConnect();
            });
        }

        public override IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new SMSDbContext());
        }
    }
}
