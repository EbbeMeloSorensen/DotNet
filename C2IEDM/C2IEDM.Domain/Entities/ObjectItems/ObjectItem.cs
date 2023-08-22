namespace C2IEDM.Domain.Entities.ObjectItems;

public class ObjectItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? AlternativeIdentificationText { get; set; }

    public ObjectItem()
    {
        Name = "";
    }
}