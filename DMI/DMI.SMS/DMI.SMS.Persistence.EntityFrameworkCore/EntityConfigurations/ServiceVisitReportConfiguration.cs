using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations
{
    public class ServiceVisitReportConfiguration : IEntityTypeConfiguration<ServiceVisitReport>
    {
        public void Configure(EntityTypeBuilder<ServiceVisitReport> builder)
        {
            builder.HasKey(_ => _.GdbArchiveOid);
        }
    }
}
