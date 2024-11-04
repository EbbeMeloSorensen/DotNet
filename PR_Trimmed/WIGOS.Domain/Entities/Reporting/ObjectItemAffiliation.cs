using WIGOS.Domain.Entities.Affiliations;
using WIGOS.Domain.Entities.ObjectItems;

namespace WIGOS.Domain.Entities.Reporting
{
    public class ObjectItemAffiliation
    {
        public Guid ObjectItemId { get; set; }
        public ObjectItem ObjectItem { get; set; } = null!;

        public Guid AffiliationId { get; set; }
        public Affiliation Affiliation { get; set; } = null!;

        public int ObjectItemAffiliationIndex { get; set; }

        public Guid ReportingDataId { get; set; }
        public ReportingData ReportingData { get; set; } = null!;
    }
}