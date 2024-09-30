using Craft.Persistence;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.Repositories
{
    public interface IServiceVisitReportRepository : IRepository<ServiceVisitReport>
    {
        int GenerateUniqueObjectId();
        string GenerateUniqueGlobalId();
    }
}
