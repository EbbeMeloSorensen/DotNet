using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Domain.Entities;

namespace C2IEDM.Persistence.EntityFrameworkCore
{
    public static class Seeding
    {
        public static void SeedDatabase(
            DbContext context)
        {
            SeedLocations(context);
        }

        private static void SeedLocations(
            DbContext context)
        {
            var now = DateTime.UtcNow;
            var delay = 0;

            var luke = new Person
            {
                FirstName = "Luke",
                Surname = "Skywalker",
                Category = "Jedi",
                Address = "Tatooine",
                Created = now + new TimeSpan(delay++)
            };

            var leia = new Person
            {
                FirstName = "Leia",
                Surname = "Organa",
                Description = "Princess",
                Address = "Alderaan",
                Created = now + new TimeSpan(delay++),
            };

            var anakin = new Person
            {
                FirstName = "Anakin",
                Surname = "Skywalker",
                Nickname = "Darth Vader",
                Category = "Jedi, Sith",
                Created = now + new TimeSpan(delay++),
            };

            var people = new List<Person>
            {
                luke,
                leia,
                anakin
            };

            var verticalDistance1 = new VerticalDistance
            {
                Dimension = 1.0
            };

            var absPoint1 = new AbsolutePoint
            {
                LatitudeCoordinate = 9.816808191409391,
                LongitudeCoordinate = 55.13953436148247,
            };

            var absPoint2 = new AbsolutePoint
            {
                LatitudeCoordinate = 10.202169964922433,
                LongitudeCoordinate = 54.847172968685214,
            };

            var absPoint3 = new AbsolutePoint
            {
                LatitudeCoordinate = 9.526866977996978,
                LongitudeCoordinate = 54.863907344714946,
            };

            var absPoint4 = new AbsolutePoint
            {
                LatitudeCoordinate = 10.951680768333889,
                LongitudeCoordinate = 54.97114299238095,
                VerticalDistance = verticalDistance1
            };

            var absPoint5 = new AbsolutePoint
            {
                LatitudeCoordinate = 11.931557544334767,
                LongitudeCoordinate = 54.90468731477757,
                VerticalDistance = verticalDistance1
            };

            var absPoint6 = new AbsolutePoint
            {
                LatitudeCoordinate = 11.96844180282638,
                LongitudeCoordinate = 54.59228432756085,
                VerticalDistance = verticalDistance1
            };

            var absPoint7 = new AbsolutePoint
            {
                LatitudeCoordinate = 10.905450050241162,
                LongitudeCoordinate = 54.715507173483466,
                VerticalDistance = verticalDistance1
            };

            var absolutePoints = new List<AbsolutePoint>{
                absPoint1,
                absPoint2,
                absPoint3,
                absPoint4,
                absPoint5,
                absPoint6,
                absPoint7};

            var line1 = new Line();
            var line2 = new Line();

            var lines = new List<Line> { line1, line2 };

            var linePoints = new List<LinePoint>
            {
                new LinePoint
                {
                    Line = line1,
                    Point = absPoint1,
                    Index = 0,
                    SequenceQuantity = 0
                },
                new LinePoint
                {
                    Line = line1,
                    Point = absPoint2,
                    Index = 1,
                    SequenceQuantity = 1
                },
                new LinePoint
                {
                    Line = line1,
                    Point = absPoint3,
                    Index = 2,
                    SequenceQuantity = 2
                },
                new LinePoint
                {
                    Line = line1,
                    Point = absPoint1,
                    Index = 3,
                    SequenceQuantity = 3
                },
                new LinePoint
                {
                    Line = line2,
                    Point = absPoint4,
                    Index = 0,
                    SequenceQuantity = 0
                },
                new LinePoint
                {
                    Line = line2,
                    Point = absPoint5,
                    Index = 1,
                    SequenceQuantity = 1
                },
                new LinePoint
                {
                    Line = line2,
                    Point = absPoint6,
                    Index = 2,
                    SequenceQuantity = 2
                },
                new LinePoint
                {
                    Line = line2,
                    Point = absPoint7,
                    Index = 3,
                    SequenceQuantity = 3
                },
                new LinePoint
                {
                    Line = line2,
                    Point = absPoint4,
                    Index = 4,
                    SequenceQuantity = 4
                }
            };

            var ellipse1 = new Ellipse
            {
                CentrePoint = absPoint1,
                FirstConjugateDiameterPoint = absPoint2,
                SecondConjugateDiameterPoint = absPoint3
            };

            var corridorArea1 = new CorridorArea
            {
                CenterLine = line1,
                WidthDimension = 1.5
            };

            var polygonArea1 = new PolygonArea
            {
                BoundingLine = line2,
            };

            var fanArea1 = new FanArea
            {
                VertexPoint = absPoint1,
                MinimumRangeDimension = 1,
                MaximumRangeDimension = 3,
                OrientationAngle = 30,
                SectorSizeAngle = 60
            };

            context.AddRange(people);

            context.Add(verticalDistance1);
            context.AddRange(absolutePoints);
            context.AddRange(lines);
            context.AddRange(linePoints);
            context.Add(ellipse1);
            context.Add(corridorArea1);
            context.Add(polygonArea1);
            context.Add(fanArea1);
            context.SaveChanges();
        }
    }
}
