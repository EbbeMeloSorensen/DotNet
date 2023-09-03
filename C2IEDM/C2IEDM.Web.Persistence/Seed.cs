using Microsoft.AspNetCore.Identity;
using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.EntityFrameworkCore;

namespace C2IEDM.Web.Persistence
{
    public class Seed
    {
        public static async Task SeedData(
            DataContext context,
            UserManager<AppUser> userManager)
        {
            await SeedUsers(context, userManager);
            await SeedPeople(context);
            await SeedLocations(context);
        }

        public static async Task SeedUsers(
            DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Jane",
                        UserName = "jane",
                        Email = "jane@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Tom",
                        UserName = "tom",
                        Email = "tom@test.com"
                    },
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedPeople(
            DataContext context)
        {
            if (!context.People.Any())
            {
                await context.People.AddRangeAsync(Seeding.GenerateListOfPeople());
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedLocations(
            DataContext context)
        {
            if (!context.Locations.Any())
            {
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

                await context.VerticalDistances.AddAsync(verticalDistance1);
                await context.AbsolutePoints.AddRangeAsync(absolutePoints);
                await context.Lines.AddRangeAsync(lines);
                await context.LinePoints.AddRangeAsync(linePoints);
                await context.Ellipses.AddAsync(ellipse1);
                await context.CorridorAreas.AddAsync(corridorArea1);
                await context.PolygonAreas.AddAsync(polygonArea1);
                await context.FanAreas.AddAsync(fanArea1);
                await context.SaveChangesAsync();
            }
        }
    }
}
