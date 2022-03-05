using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence
{
    public static class PositionExtensions
    {
        public static Position Clone(
            this Position position)
        {
            var clone = new Position();
            clone.CopyAttributes(position);
            return clone;
        }

        public static void CopyAttributes(
            this Position position,
            Position other)
        {
            position.Id = other.Id;
            position.StatID = other.StatID;
            position.StartTime = other.StartTime;
            position.EndTime = other.EndTime;
            position.Long = other.Long;
            position.Lat = other.Lat;
            position.Height = other.Height;
        }
    }
}
