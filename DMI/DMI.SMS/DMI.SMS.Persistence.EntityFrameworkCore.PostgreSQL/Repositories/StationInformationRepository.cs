using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.PostgreSQL.Repositories
{
    public class StationInformationRepository : Repository<StationInformation>, IStationInformationRepository
    {
        public StationInformationRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(StationInformation entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public void RemoveLogically(StationInformation entity, DateTime transactionTime)
        {
            throw new NotImplementedException();
        }

        public void Supersede(StationInformation entity, DateTime transactionTime, string user)
        {
            throw new NotImplementedException();
        }

        public StationInformation Get(int id)
        {
            throw new NotImplementedException();
        }

        public StationInformation GetStationInformationWithContactPersons(int id)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueObjectId()
        {
            var context = Context as SMSDbContext;

            if (context == null)
            {
                throw new InvalidCastException();
            }

            if (!context.StationInformations.Any())
            {
                return 1;
            }

            return context.StationInformations.Max(_ => _.ObjectId) + 1;
        }

        public string GenerateUniqueGlobalId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
