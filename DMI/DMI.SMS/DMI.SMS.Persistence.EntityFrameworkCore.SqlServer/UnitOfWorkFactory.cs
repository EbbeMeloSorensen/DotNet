using Craft.Logging;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWorkFactory : UnitOfWorkFactoryBase
    {
        static UnitOfWorkFactory()
        {
            using var context = new SMSDbContext();
            context.Database.EnsureCreated();

            //if (context.StationInformations.Any()) return;

            //SeedDatabase(context);
        }

        public override void Initialize(ILogger logger)
        {
        }

        public override Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public override IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new SMSDbContext());
        }

        private static void SeedDatabase(SMSDbContext context)
        {
            var stationInformations = new List<StationInformation>
            {
                new StationInformation
                {
                    StationName = "Livgardens Kaserne"
                }
            };

            context.StationInformations.AddRange(stationInformations);
            context.SaveChanges();
        }
    }
}
