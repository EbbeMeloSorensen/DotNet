using WIGOS.Domain.Entities.ObjectItems.Organisations;

namespace WIGOS.Domain.Entities.Context
{
    public enum OperationalInformationGroupOrganisationAssociationCategoryCode
    {
        IsUnderOperationalResponsibilityOf,
        IsUnderProxyResponsibilityOf
    }

    public class OperationalInformationGroupOrganisationAssociation
    {
        public Guid OperationalInformationGroupId { get; set; }
        public OperationalInformationGroup OperationalInformationGroup { get; set; } = null!;

        public Guid OrganisationId { get; set; }
        public Organisation Organisation { get; set; } = null!;

        public int Index { get; set; }

        public OperationalInformationGroupOrganisationAssociationCategoryCode OperationalInformationGroupOrganisationAssociationCategoryCode { get; set; }
    }
}