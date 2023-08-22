using C2IEDM.Domain.Entities.ObjectItems;
using Domain.Entities.Reporting;

namespace C2IEDM.Domain.Entities.Reporting;

public enum ReportingDataAccuracyCode
{
    Confirmed,
    Doubtful,
    Improbable,
    Possible,
    Probable,
    TruthCannotBeJudged
}

public enum ReportingDataCategoryCode
{
    Assumed,
    Erroneous,
    Extrapolated,
    Inferred,
    Planned,
    Reported
}

public enum ReportingDataCredibilityCode
{
    Indeterminate,
    ReportedAsAFact,
    ReportedAsPlausible,
    ReportedAsUncertain
}

public enum ReportingDataReliabilityCode
{
    CompletelyReliable,
    FairlyReliable,
    NotUsuallyReliable,
    ReliabilityCannotBeJudged,
    Unreliable,
    UsuallyReliable
}

public enum ReportingDataSourceTypeCode
{
    AirReconnaissance,
    CapturedMaterial,
    ElectronicIntelligence,
    PrisonerOfWar,
    UnattendedGroundSensor
}

public abstract class ReportingData
{
    public Guid Id { get; set; }

    public Guid ReportingOrganisationId { get; set; }
    public Organisation ReportingOrganisation { get; set; } = null!;

    public Guid? ReferenceId { get; set; }
    public virtual Reference? Reference { get; set; }

    public ReportingDataAccuracyCode ReportingDataAccuracyCode { get; set; }
    public ReportingDataCategoryCode ReportingDataCategoryCode { get; set; }
    public ReportingDataCredibilityCode ReportingDataCredibilityCode { get; set; }
    public ReportingDataReliabilityCode ReportingDataReliabilityCode { get; set; }
    public ReportingDataSourceTypeCode? ReportingDataSourceTypeCode { get; set; }

    public DateTime ReportingDateTime { get; set; }
}