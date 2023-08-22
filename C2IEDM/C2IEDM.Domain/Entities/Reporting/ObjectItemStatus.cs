using C2IEDM.Domain.Entities.ObjectItems;

namespace C2IEDM.Domain.Entities.Reporting;

public enum ObjectItemStatusHostilityCode
{
    AssumedFriend,
    AssumedHostile,
    AssumedInvolved,
    AssumedNeutral,
    Friend,
    Hostile,
    Involved,
    Neutral,
    Faker,
    Joker,
    Pending,
    Suspect,
    Unidentified,
    Unknown
}

public enum ObjectItemStatusBoobyTrapIndicatorCode
{
    No,
    Unknown,
    Yes
}

public abstract class ObjectItemStatus
{
    public Guid ObjectItemId { get; set; }
    public ObjectItem ObjectItem { get; set; } = null!;

    public int ObjectItemStatusIndex { get; set; }

    public Guid ReportingDataId { get; set; }
    public ReportingData ReportingData { get; set; } = null!;

    public ObjectItemStatusHostilityCode ObjectItemStatusHostilityCode { get; set; }
    public ObjectItemStatusBoobyTrapIndicatorCode ObjectItemStatusBoobyTrapIndicatorCode { get; set; }
}