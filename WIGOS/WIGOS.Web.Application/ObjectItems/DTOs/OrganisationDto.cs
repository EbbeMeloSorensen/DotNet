namespace WIGOS.Web.Application.ObjectItems.DTOs
{
    public class OrganisationDto : ObjectItemDto
    {
        public string? NickName { get; set; }

        public OrganisationDto()
        {
            type = "Organisation";
        }
    }
}