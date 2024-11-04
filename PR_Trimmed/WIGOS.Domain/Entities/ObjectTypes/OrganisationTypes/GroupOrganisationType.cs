namespace WIGOS.Domain.Entities.ObjectTypes.OrganisationTypes
{
    public enum GroupOrganisationTypeCategoryCode
    {
        DisplacedPerson,
        Landowner,
        Local,
        Media,
        MediaInternational,
        Merchant,
        Refugee,
        Writer
    }

    public class GroupOrganisationType : OrganisationType
    {
        public GroupOrganisationTypeCategoryCode GroupOrganisationTypeCategoryCode { get; set; }

        public GroupOrganisationType() : base()
        {
            OrganisationTypeCategory = OrganisationTypeCategory.GroupOrganisationType;
        }
    }
}