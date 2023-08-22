using C2IEDM.Domain.Entities.Reporting;

namespace C2IEDM.Domain.Entities.Context;

public class ContextElement
{
    public Guid ContextId { get; set; }
    public Context Context { get; set; } = null!;
    public int Index { get; set; }

    public Guid ReportingDataId { get; set; }
    public ReportingData ReportingData { get; set; } = null!;
}