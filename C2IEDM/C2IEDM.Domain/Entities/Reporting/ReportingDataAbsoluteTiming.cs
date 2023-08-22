namespace C2IEDM.Domain.Entities.Reporting;

public class ReportingDataAbsoluteTiming : ReportingData
{
    public DateTime EffectiveStartDateTime { get; set; }
    public DateTime? EffectiveEndDateTime { get; set; }
}