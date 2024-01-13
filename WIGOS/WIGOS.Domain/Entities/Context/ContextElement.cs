using WIGOS.Domain.Entities.Reporting;

namespace WIGOS.Domain.Entities.Context
{
    public class ContextElement
    {
        public Guid ContextId { get; set; }
        public Context Context { get; set; } = null!;
        public int Index { get; set; }

        public Guid ReportingDataId { get; set; }
        public ReportingData ReportingData { get; set; } = null!;
    }
}