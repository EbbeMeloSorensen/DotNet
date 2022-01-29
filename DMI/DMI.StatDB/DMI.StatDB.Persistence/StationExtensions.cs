using System.Linq;
using System.Collections.Generic;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence
{
    public static class StationExtensions
    {
        public static Station Clone(
            this Station station)
        {
            var clone = new Station();
            clone.CopyAttributes(station);
            return clone;
        }

        public static void CopyAttributes(
            this Station station,
            Station other)
        {
            station.StatID = other.StatID;
            station.IcaoId = other.IcaoId;
            station.Country = other.Country;
            station.Source = other.Source;

            if (other.Positions != null && other.Positions.Any())
            {
                station.Positions = new HashSet<Position>();

                foreach (var position in other.Positions)
                {
                    station.Positions.Add(position.Clone());
                }
            }
        }
    }
}
