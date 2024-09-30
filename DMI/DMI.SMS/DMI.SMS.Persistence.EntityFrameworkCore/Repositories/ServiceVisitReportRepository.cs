using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Repositories
{
    public class ServiceVisitReportRepository : Repository<ServiceVisitReport>, IServiceVisitReportRepository
    {
        public ServiceVisitReportRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(ServiceVisitReport entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<ServiceVisitReport> entities)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueObjectId()
        {
            throw new NotImplementedException();
        }

        public string GenerateUniqueGlobalId()
        {
            throw new NotImplementedException();
        }
    }
}
