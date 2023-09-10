namespace C2IEDM.Domain.Entities.ObjectItems;

public class ObjectItem : VersionedObject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? AlternativeIdentificationText { get; set; }

    public ObjectItem(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
        Name = "";
    }
}