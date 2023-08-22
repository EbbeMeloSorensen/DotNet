namespace C2IEDM.Domain.Entities.ObjectTypes.FacilityTypes;

public enum BridgeTypeDesignTypeCode
{
    BoxGirder,
    Suspension,
    Truss
}

public class BridgeType : FacilityType
{
    public BridgeTypeDesignTypeCode BridgeTypeDesignTypeCode { get; set; }

    public BridgeType() : base()
    {
        FacilityTypeCategoryCode = FacilityTypeCategoryCode.BridgeType;
    }
}