using DD.Domain;

namespace DD.Persistence
{
    public static class CreatureTypeExtensions
    {
        public static CreatureType Clone(
            this CreatureType creatureType)
        {
            var clone = new CreatureType();
            clone.CopyAttributes(creatureType);
            return clone;
        }

        public static void CopyAttributes(
            this CreatureType creatureType,
            CreatureType other)
        {
            creatureType.Id = other.Id;
            creatureType.Name = other.Name;
            creatureType.InitiativeModifier = other.InitiativeModifier;
            creatureType.ArmorClass = other.ArmorClass;
            creatureType.MaxHitPoints = other.MaxHitPoints;
            creatureType.Movement = other.Movement;
            creatureType.Thaco = other.Thaco;
        }
    }
}
