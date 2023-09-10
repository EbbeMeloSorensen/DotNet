namespace C2IEDM.Web.Application.ObjectItems.DTOs;

public class UnitDto : OrganisationDto
{
    public string FormalAbbreviatedName { get; set; }

    public UnitDto()
    {
        type = "Unit";
    }
}