namespace C2IEDM.Domain.Entities.ObjectTypes.FacilityTypes;

public enum MilitaryObstacleTypeCategoryCode
{
    DragonTeeth,
    Roadblock,
    Tetrahedron
}

public class MilitaryObstacleType : FacilityType
{
    public MilitaryObstacleTypeCategoryCode MilitaryObstacleTypeCategoryCode { get; set; }

    public MilitaryObstacleType() : base()
    {
        FacilityTypeCategoryCode = FacilityTypeCategoryCode.MilitaryObstacleType;
    }
}