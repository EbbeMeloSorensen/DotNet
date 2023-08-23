using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;

namespace C2IEDM.Persistence.EntityFrameworkCore
{
    public static class Seeding
    {
        public static void SeedDatabase(
            DbContext context)
        {
            var location1 = new AbsolutePoint
            {
                LatitudeCoordinate = 23.1,
                LongitudeCoordinate = 34.1
            };

            var location2 = new AbsolutePoint
            {
                LatitudeCoordinate = 23.2,
                LongitudeCoordinate = 34.2
            };

            var locations = new List<Location>
            {
                location1,
                location2
            };

            context.AddRange(locations);
            context.SaveChanges();
        }
    }
}
