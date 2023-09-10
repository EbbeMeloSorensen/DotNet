namespace C2IEDM.Web.Application.ObjectItems.DTOs;

public class ObjectItemDto
{
    public string type { get; set; }
    public Guid id { get; set; }
    public string Name { get; set; }
    public string? AlternativeIdentificationText { get; set; }

    public ObjectItemDto()
    {
        type = "Object Item";
    }
}