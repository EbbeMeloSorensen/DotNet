using WIGOS.Domain.Entities.ObjectItems;
using WIGOS.Domain.Entities.ObjectTypes;

namespace WIGOS.Domain.Entities.Reporting
{
    public class ObjectItemType
    {
        public Guid ObjectItemId { get; set; }
        public ObjectItem ObjectItem { get; set; } = null!;

        public Guid ObjectTypeId { get; set; }
        public ObjectType ObjectType { get; set; } = null!;

        public int ObjectItemTypeIndex { get; set; }

        public Guid ReportingDataId { get; set; }
        public ReportingData ReportingData { get; set; } = null!;
    }
}