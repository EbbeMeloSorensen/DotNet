using WIGOS.Domain.Entities.Geometry;
using WIGOS.Domain.Entities.Geometry.CoordinateSystems;
using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Domain.Entities.ObjectItems;
using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using WIGOS.Persistence.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WIGOS.Web.Persistence
{
    public class Seed
    {
        private static DateTime _now;
        private static int _delayInTicks;

        static Seed()
        {
            _now = DateTime.UtcNow;
            _delayInTicks = 0;
        }

        public static async Task SeedData(
            DataContext context,
            UserManager<AppUser> userManager)
        {
            await SeedUsers(context, userManager);
            await SeedPeople(context);
            await SeedLocations(context);
            await SeedObjectItems(context);
            await SeedWigosStuff(context);
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
                var createdTime = DateTime.UtcNow;
                var delayInTicks = 0;

                var verticalDistance1 = new VerticalDistance(
                    Guid.NewGuid(),
                    NextCreatedTime())
                {
                    Dimension = 1.0,
                };

                var absPoint1 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 9.816808191409391,
                    LongitudeCoordinate = 55.13953436148247,
                };

                var absPoint2 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 10.202169964922433,
                    LongitudeCoordinate = 54.847172968685214,
                };

                var absPoint3 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 9.526866977996978,
                    LongitudeCoordinate = 54.863907344714946,
                };

                var absPoint4 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 10.951680768333889,
                    LongitudeCoordinate = 54.97114299238095,
                    VerticalDistance = verticalDistance1,
                    VerticalDistanceObjectId = verticalDistance1.ObjectId
                };

                var absPoint5 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 11.931557544334767,
                    LongitudeCoordinate = 54.90468731477757,
                    VerticalDistance = verticalDistance1,
                    VerticalDistanceObjectId = verticalDistance1.ObjectId
                };

                var absPoint6 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 11.96844180282638,
                    LongitudeCoordinate = 54.59228432756085,
                    VerticalDistance = verticalDistance1,
                    VerticalDistanceObjectId = verticalDistance1.ObjectId
                };

                var absPoint7 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    LatitudeCoordinate = 10.905450050241162,
                    LongitudeCoordinate = 54.715507173483466,
                    VerticalDistance = verticalDistance1,
                    VerticalDistanceObjectId = verticalDistance1.ObjectId
                };

                var absolutePoints = new List<AbsolutePoint>{
                    absPoint1,
                    absPoint2,
                    absPoint3,
                    absPoint4,
                    absPoint5,
                    absPoint6,
                    absPoint7};

                var line1 = new Line(Guid.NewGuid(), NextCreatedTime());
                var line2 = new Line(Guid.NewGuid(), NextCreatedTime());

                var lines = new List<Line> { line1, line2 };

                var linePoints = new List<LinePoint>
                {
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line1,
                        LineObjectId = line1.ObjectId,
                        Point = absPoint1,
                        PointObjectId = absPoint1.ObjectId,
                        Index = 0,
                        SequenceQuantity = 0
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line1,
                        LineObjectId = line1.ObjectId,
                        Point = absPoint2,
                        PointObjectId = absPoint2.ObjectId,
                        Index = 1,
                        SequenceQuantity = 1
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line1,
                        LineObjectId = line1.ObjectId,
                        Point = absPoint3,
                        PointObjectId = absPoint3.ObjectId,
                        Index = 2,
                        SequenceQuantity = 2
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line1,
                        LineObjectId = line1.ObjectId,
                        Point = absPoint1,
                        PointObjectId = absPoint1.ObjectId,
                        Index = 3,
                        SequenceQuantity = 3
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line2,
                        LineObjectId = line2.ObjectId,
                        Point = absPoint4,
                        PointObjectId = absPoint4.ObjectId,
                        Index = 0,
                        SequenceQuantity = 0
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line2,
                        LineObjectId = line2.ObjectId,
                        Point = absPoint5,
                        PointObjectId = absPoint5.ObjectId,
                        Index = 1,
                        SequenceQuantity = 1
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line2,
                        LineObjectId = line2.ObjectId,
                        Point = absPoint6,
                        PointObjectId = absPoint6.ObjectId,
                        Index = 2,
                        SequenceQuantity = 2
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line2,
                        LineObjectId = line2.ObjectId,
                        Point = absPoint7,
                        PointObjectId = absPoint7.ObjectId,
                        Index = 3,
                        SequenceQuantity = 3
                    },
                    new LinePoint(Guid.NewGuid(), NextCreatedTime())
                    {
                        Line = line2,
                        LineObjectId = line2.ObjectId,
                        Point = absPoint4,
                        PointObjectId = absPoint4.ObjectId,
                        Index = 4,
                        SequenceQuantity = 4
                    }
                };

                //    var ellipse1 = new Ellipse(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        CentrePoint = absPoint1,
                //        FirstConjugateDiameterPoint = absPoint2,
                //        SecondConjugateDiameterPoint = absPoint3
                //    };

                //    var corridorArea1 = new CorridorArea(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        CenterLine = line1,
                //        WidthDimension = 1.5
                //    };

                //    var polygonArea1 = new PolygonArea(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        BoundingLine = line2,
                //    };

                //    var fanArea1 = new FanArea(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        VertexPoint = absPoint1,
                //        MinimumRangeDimension = 1,
                //        MaximumRangeDimension = 3,
                //        OrientationAngle = 30,
                //        SectorSizeAngle = 60
                //    };

                //    var pointReference1 = new PointReference(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        OriginPoint = absPoint1,
                //        XVectorPoint = absPoint2,
                //        YVectorPoint = absPoint3
                //    };

                //    var pointReference2 = new PointReference(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        OriginPoint = absPoint2,
                //        XVectorPoint = absPoint3,
                //        YVectorPoint = absPoint4
                //    };

                //    var pointReferences = new List<PointReference>
                //    {
                //        pointReference1, pointReference2
                //    };

                //    var relativePoint1 = new RelativePoint(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        CoordinateSystem = pointReference1,
                //        XCoordinateDimension = 1.2,
                //        YCoordinateDimension = 3.4,
                //        ZCoordinateDimension = 5.6
                //    };

                //    var relativePoint2 = new RelativePoint(Guid.NewGuid(), NextCreatedTime())
                //    {
                //        CoordinateSystem = pointReference1,
                //        XCoordinateDimension = 2.3,
                //        YCoordinateDimension = 4.5,
                //        ZCoordinateDimension = 6.7
                //    };

                //    var relativePoints = new List<RelativePoint>
                //    {
                //        relativePoint1, relativePoint2
                //    };

                await context.VerticalDistances.AddAsync(verticalDistance1);
                await context.AbsolutePoints.AddRangeAsync(absolutePoints);
                await context.Lines.AddRangeAsync(lines);
                await context.LinePoints.AddRangeAsync(linePoints);
                //await context.Ellipses.AddAsync(ellipse1);
                //await context.CorridorAreas.AddAsync(corridorArea1);
                //await context.PolygonAreas.AddAsync(polygonArea1);
                //await context.FanAreas.AddAsync(fanArea1);
                //await context.PointReferences.AddRangeAsync(pointReferences);
                //await context.RelativePoints.AddRangeAsync(relativePoints);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedObjectItems(
            DataContext context)
        {
            if (!context.ObjectItems.Any())
            {
                var objectItem1 = new ObjectItem(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Bamse"
                };

                var objectItem2 = new ObjectItem(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Kylling"
                };

                var objectItems = new List<ObjectItem>
                {
                    objectItem1, objectItem2
                };

                var organisation1 = new Organisation(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Luna",
                    NickName = "L"
                };

                var organisation2 = new Organisation(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Aske",
                    NickName = "A"
                };

                var organisations = new List<Organisation>
                {
                    organisation1, organisation2
                };

                var unit1 = new Unit(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Forlæns",
                    NickName = "F",
                    FormalAbbreviatedName = "Fx"
                };

                var unit2 = new Unit(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Baglæns",
                    NickName = "B",
                    FormalAbbreviatedName = "Bx"
                };

                var units = new List<Unit>
                {
                    unit1, unit2
                };

                await context.ObjectItems.AddRangeAsync(objectItems);
                await context.Organisations.AddRangeAsync(organisations);
                await context.Units.AddRangeAsync(units);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedWigosStuff(
            DataContext context)
        {
            if (!context.AbstractEnvironmentalMonitoringFacilities.Any())
            {
                var observingFacility1 = new ObservingFacility(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Livgardens Kaserne",
                    DateEstablished = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };

                var observingFacility2 = new ObservingFacility(Guid.NewGuid(), NextCreatedTime())
                {
                    Name = "Esbjerg Havn",
                    DateEstablished = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    DateClosed = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };

                var observingFacilities = new List<ObservingFacility>
                {
                    observingFacility1, observingFacility2
                };

                var point1 = new Domain.Entities.WIGOS.GeospatialLocations.Point(Guid.NewGuid(), NextCreatedTime())
                {
                    From = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    To = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CoordinateSystem = "WGS_84",
                    Coordinate1 = 1.2,
                    Coordinate2 = 3.4,
                    AbstractEnvironmentalMonitoringFacility = observingFacility1,
                    AbstractEnvironmentalMonitoringFacilityObjectId = observingFacility1.ObjectId
                };

                var point2 = new Domain.Entities.WIGOS.GeospatialLocations.Point(Guid.NewGuid(), NextCreatedTime())
                {
                    From = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CoordinateSystem = "WGS_84",
                    Coordinate1 = 5.6,
                    Coordinate2 = 7.8,
                    AbstractEnvironmentalMonitoringFacility = observingFacility1,
                    AbstractEnvironmentalMonitoringFacilityObjectId = observingFacility1.ObjectId
                };

                var point3 = new Domain.Entities.WIGOS.GeospatialLocations.Point(Guid.NewGuid(), NextCreatedTime())
                {
                    From = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CoordinateSystem = "WGS_84",
                    Coordinate1 = 3.4,
                    Coordinate2 = 5.6,
                    AbstractEnvironmentalMonitoringFacility = observingFacility2,
                    AbstractEnvironmentalMonitoringFacilityObjectId = observingFacility2.ObjectId
                };

                var points = new List<Domain.Entities.WIGOS.GeospatialLocations.Point>
                {
                    point1, point2, point3
                };

                await context.ObservingFacilities.AddRangeAsync(observingFacilities);
                await context.Points_WIGOS.AddRangeAsync(points);
                await context.SaveChangesAsync();
            }
        }

        private static DateTime NextCreatedTime()
        {
            _delayInTicks += 1000;
            return _now + new TimeSpan(_delayInTicks);
        }
    }
}
