namespace WIGOS.Domain.Entities.ObjectTypes.OrganisationTypes
{
    public enum OrganisationTypeCategory
    {
        NotOtherwiseSpecified,
        CivilianPostType,
        GovernmentOrganisationType,
        GroupOrganisationType,
        PrivateSectorOrganisationType
    }

    public enum CommandAndControlCategory
    {
        NotOtherwiseSpecified,
        NotKnown,
        CommandPost,
        CoordinationCentre,
        HeadQuarters,
        OperationsCentre
    }

    public class OrganisationType : ObjectType
    {
        public OrganisationTypeCategory OrganisationTypeCategory { get; set; }
        public CommandAndControlCategory? CommandAndControlCategory { get; set; }
        public bool CommandFunctionIndicatorCode { get; set; }
        public string DescriptionText { get; set; }

        public OrganisationType() : base()
        {
            ObjectTypeCategoryCode = ObjectTypeCategoryCode.OrganisationType;
            DescriptionText = "";
        }
    }
}