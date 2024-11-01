namespace WIGOS.Domain.Entities.ObjectTypes.FacilityTypes
{
    public enum FacilityTypeCategoryCode
    {
        NotOtherwiseSpecified,
        BridgeType,
        MilitaryObstacleType,
        DecontaminationFacility,
        FuelHandlingPoint,
        MaintenanceFacility,
        SupplyDump,
        DepotNotOtherwiseSpecified,
        AirfieldAirportAirstrip
    }

    public class FacilityType : ObjectType
    {
        public FacilityTypeCategoryCode FacilityTypeCategoryCode { get; set; }

        public FacilityType() : base()
        {
            ObjectTypeCategoryCode = ObjectTypeCategoryCode.FacilityType;
        }
    }
}