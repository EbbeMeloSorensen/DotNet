using Microsoft.EntityFrameworkCore;
using WIGOS.Domain.Entities;
using WIGOS.Domain.Entities.Geometry;
using WIGOS.Domain.Entities.Geometry.CoordinateSystems;
using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace WIGOS.Persistence.EntityFrameworkCore
{
    public static class Seeding
    {
        private static DateTime _now;
        private static int _delayInTicks;

        static Seeding()
        {
            _now = DateTime.UtcNow;
        }

        public static void SeedDatabase(
            WIGOSDbContextBase context)
        {
            if (context.ObservingFacilities.Any() ||
                context.Locations.Any())
            {
                return;
            }

            SeedLocations(context);
            SeedPeople(context);
            SeedObservingFacilities(context);
        }

        public static IEnumerable<Person> GenerateListOfPeople()
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

            var obiWan = new Person
            {
                FirstName = "Obi-Wan",
                Surname = "Kenobi",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var padme = new Person
            {
                FirstName = "Padme",
                Surname = "Amidala",
                Address = "Naboo",
                Description = "Princess",
                Created = now + new TimeSpan(delay++),
            };

            var benSolo = new Person
            {
                FirstName = "Ben",
                Surname = "Solo",
                Nickname = "Kylo Ren",
                Category = "Jedi/Sith",
                Created = now + new TimeSpan(delay++),
            };

            var hanSolo = new Person
            {
                FirstName = "Han",
                Surname = "Solo",
                Description = "Smuggler",
                Created = now + new TimeSpan(delay++),
            };

            var quiGon = new Person
            {
                FirstName = "Qui-Gon",
                Surname = "Jinn",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var countDooku = new Person
            {
                FirstName = "Count Dooku",
                Nickname = "Darth Tyranus",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var palpatine = new Person
            {
                FirstName = "Emperor Palpatine",
                Nickname = "Darth Sidious",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var rey = new Person
            {
                FirstName = "Rey",
                Surname = "Skywalker",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var maul = new Person
            {
                FirstName = "Darth Maul",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var plagueis = new Person
            {
                FirstName = "Darth Plagueis",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var yoda = new Person
            {
                FirstName = "Yoda",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var lando = new Person
            {
                FirstName = "Lando",
                Surname = "Calrissian",
                Description = "Smuggler",
                Created = now + new TimeSpan(delay++),
            };

            var chewbacca = new Person
            {
                FirstName = "Chewbacca",
                Nickname = "Chewie",
                Address = "Kashyyyk",
                Description = "Wookie",
                Created = now + new TimeSpan(delay++),
            };

            var bobaFett = new Person
            {
                FirstName = "Boba",
                Surname = "Fett",
                Address = "Mandalore",
                Description = "Bounty Hunter",
                Created = now + new TimeSpan(delay++),
            };

            var tarkin = new Person
            {
                FirstName = "Wilhuff",
                Surname = "Tarkin",
                Nickname = "Grand Moff Tarkin",
                Created = now + new TimeSpan(delay++),
            };

            var maxRebo = new Person
            {
                FirstName = "Max",
                Surname = "Rebo",
                Created = now + new TimeSpan(delay++),
            };

            var nienNunb = new Person
            {
                FirstName = "Nien",
                Surname = "Nunb",
                Created = now + new TimeSpan(delay++),
            };

            var bibFortuna = new Person
            {
                FirstName = "Bib",
                Surname = "Fortuna",
                Created = now + new TimeSpan(delay++),
            };

            var admiralAckbar = new Person
            {
                FirstName = "Ackbar",
                Nickname = "Admiral Ackbar",
                Created = now + new TimeSpan(delay++),
            };

            var wicket = new Person
            {
                FirstName = "Wicket",
                Created = now + new TimeSpan(delay++),
            };

            var oola = new Person
            {
                FirstName = "Oola",
                Created = now + new TimeSpan(delay++),
            };

            var yarnaDalGargan = new Person
            {
                FirstName = "Yarna",
                Surname = "d'al Gargan",
                Created = now + new TimeSpan(delay++),
            };

            var dengar = new Person
            {
                FirstName = "Dengar",
                Description = "Bounty hunter",
                Created = now + new TimeSpan(delay++),
            };

            var bossk = new Person
            {
                FirstName = "Bossk",
                Description = "Bounty hunter",
                Created = now + new TimeSpan(delay++),
            };

            var shmi = new Person
            {
                FirstName = "Shmi",
                Surname = "Skywalker",
                Address = "Tatooine",
                Created = now + new TimeSpan(delay++),
            };

            var c3po = new Person
            {
                FirstName = "C-3PO",
                Created = now + new TimeSpan(delay++),
            };

            var r2d2 = new Person
            {
                FirstName = "R2-D2",
                Created = now + new TimeSpan(delay++),
            };

            var jarjar = new Person
            {
                FirstName = "Jar Jar",
                Surname = "Binks",
                Created = now + new TimeSpan(delay++),
            };

            var generalGrievous = new Person
            {
                FirstName = "Qymaen",
                Surname = " jai Sheelal",
                Nickname = "General Grievous",
                Created = now + new TimeSpan(delay++),
            };

            var maceWindu = new Person
            {
                FirstName = "Mace",
                Surname = "Windu",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var kiAdiMundi = new Person
            {
                FirstName = "Ki-Adi-Mundi",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var agenKolar = new Person
            {
                FirstName = "Agen",
                Surname = "Kolar",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var saeseeTiin = new Person
            {
                FirstName = "Saesee",
                Surname = "Tin",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var kitFisto = new Person
            {
                FirstName = "Kit",
                Category = "Fisto",
                Created = now + new TimeSpan(delay++),
            };

            var jabba = new Person
            {
                FirstName = "Jabba",
                Surname = "Desilijic Tiure",
                Nickname = "Jabba the Hutt",
                Created = now + new TimeSpan(delay++),
            };

            var monMothma = new Person
            {
                FirstName = "Mon",
                Surname = "Mothma",
                Created = now + new TimeSpan(delay++),
            };

            var jynErso = new Person
            {
                FirstName = "Jyn",
                Surname = "Erso",
                Created = now + new TimeSpan(delay++),
            };

            var galenErso = new Person
            {
                FirstName = "Galen",
                Surname = "Erso",
                Created = now + new TimeSpan(delay++),
            };

            var orsonKrennic = new Person
            {
                FirstName = "Orson",
                Surname = "Krennic",
                Created = now + new TimeSpan(delay++),
            };

            var chirrutImwe = new Person
            {
                FirstName = "Chirrut",
                Surname = "Imwe",
                Created = now + new TimeSpan(delay++),
            };

            var bazeMalbus = new Person
            {
                FirstName = "Baze",
                Surname = "Malbus",
                Created = now + new TimeSpan(delay++),
            };

            var k2so = new Person
            {
                FirstName = "K-2SO",
                Created = now + new TimeSpan(delay++),
            };

            var cassian = new Person
            {
                FirstName = "Cassian",
                Surname = "Andor",
                Created = now + new TimeSpan(delay++),
            };

            var sawGerrera = new Person
            {
                FirstName = "Saw",
                Surname = "Gerrera",
                Created = now + new TimeSpan(delay++),
            };

            var luthen = new Person
            {
                FirstName = "Luthen",
                Surname = "Rael",
                Created = now + new TimeSpan(delay++),
            };

            var maarva = new Person
            {
                FirstName = "Maarva",
                Surname = "Andor",
                Created = now + new TimeSpan(delay++),
            };

            var tobiasBeckett = new Person
            {
                FirstName = "Tobias",
                Surname = "Beckett",
                Created = now + new TimeSpan(delay++),
            };

            var qira = new Person
            {
                FirstName = "Qi´ra",
                Created = now + new TimeSpan(delay++),
            };

            var l337 = new Person
            {
                FirstName = "L3-37",
                Created = now + new TimeSpan(delay++),
            };

            var drydenVos = new Person
            {
                FirstName = "Dryden",
                Surname = "Vos",
                Created = now + new TimeSpan(delay++),
            };

            var bb8 = new Person
            {
                FirstName = "BB-8",
                Created = now + new TimeSpan(delay++),
            };

            var poeDameron = new Person
            {
                FirstName = "Poe",
                Surname = "Dameron",
                Created = now + new TimeSpan(delay++),
            };

            var finn = new Person
            {
                FirstName = "Finn",
                Created = now + new TimeSpan(delay++),
            };

            var mazKanata = new Person
            {
                FirstName = "Maz",
                Surname = "Kanata",
                Created = now + new TimeSpan(delay++),
            };

            var ahsoka = new Person
            {
                FirstName = "Ahsoka",
                Surname = "Tano",
                Created = now + new TimeSpan(delay++),
            };

            var lorSanTekka = new Person
            {
                FirstName = "Lor San",
                Surname = "Tekka",
                Created = now + new TimeSpan(delay++),
            };

            var roseTico = new Person
            {
                FirstName = "Rose",
                Surname = "Tico",
                Created = now + new TimeSpan(delay++),
            };

            var bailOrgana = new Person
            {
                FirstName = "Bail",
                Surname = "Organa",
                Created = now + new TimeSpan(delay++),
            };

            var dinDjarin = new Person
            {
                FirstName = "Din",
                Surname = "Djarin",
                Nickname = "Mando",
                Created = now + new TimeSpan(delay++),
            };

            var kuiil = new Person
            {
                FirstName = "Kuiil",
                Created = now + new TimeSpan(delay++),
            };

            var grogu = new Person
            {
                FirstName = "Grogu",
                Created = now + new TimeSpan(delay++),
            };

            var boKatanKryze = new Person
            {
                FirstName = "Bo-Katan",
                Surname = "Kryze",
                Created = now + new TimeSpan(delay++),
            };

            var moffGideon = new Person
            {
                FirstName = "Gideon",
                Nickname = "Moff Gideon",
                Created = now + new TimeSpan(delay++),
            };

            var people = new List<Person>
            {
                luke,
                leia,
                anakin,
                obiWan,
                padme,
                benSolo,
                hanSolo,
                quiGon,
                countDooku,
                palpatine,
                rey,
                maul,
                plagueis,
                yoda,
                lando,
                chewbacca,
                bobaFett,
                tarkin,
                maxRebo,
                nienNunb,
                bibFortuna,
                admiralAckbar,
                wicket,
                oola,
                yarnaDalGargan,
                dengar,
                bossk,
                shmi,
                c3po,
                r2d2,
                jarjar,
                generalGrievous,
                maceWindu,
                kiAdiMundi,
                agenKolar,
                saeseeTiin,
                kitFisto,
                jabba,
                monMothma,
                jynErso,
                galenErso,
                orsonKrennic,
                chirrutImwe,
                bazeMalbus,
                k2so,
                cassian,
                luthen,
                sawGerrera,
                maarva,
                tobiasBeckett,
                qira,
                l337,
                drydenVos,
                bb8,
                poeDameron,
                finn,
                mazKanata,
                ahsoka,
                lorSanTekka,
                roseTico,
                bailOrgana,
                dinDjarin,
                kuiil,
                grogu,
                boKatanKryze,
                moffGideon
            };

            return people;
        }

        private static void SeedPeople(
            DbContext context)
        {
        }

        private static void SeedLocations(
            DbContext context)
        {
            var verticalDistance1 = new VerticalDistance(Guid.NewGuid(), NextCreatedTime())
            {
                Dimension = 1.0
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
                VerticalDistance = verticalDistance1
            };

            var absPoint5 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
            {
                LatitudeCoordinate = 11.931557544334767,
                LongitudeCoordinate = 54.90468731477757,
                VerticalDistance = verticalDistance1
            };

            var absPoint6 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
            {
                LatitudeCoordinate = 11.96844180282638,
                LongitudeCoordinate = 54.59228432756085,
                VerticalDistance = verticalDistance1
            };

            var absPoint7 = new AbsolutePoint(Guid.NewGuid(), NextCreatedTime())
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

            var line1 = new Line(Guid.NewGuid(), NextCreatedTime());
            var line2 = new Line(Guid.NewGuid(), NextCreatedTime());

            var lines = new List<Line> { line1, line2 };

            var linePoints = new List<LinePoint>
            {
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line1,
                    Point = absPoint1,
                    Index = 0,
                    SequenceQuantity = 0
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line1,
                    Point = absPoint2,
                    Index = 1,
                    SequenceQuantity = 1
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line1,
                    Point = absPoint3,
                    Index = 2,
                    SequenceQuantity = 2
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line1,
                    Point = absPoint1,
                    Index = 3,
                    SequenceQuantity = 3
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line2,
                    Point = absPoint4,
                    Index = 0,
                    SequenceQuantity = 0
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line2,
                    Point = absPoint5,
                    Index = 1,
                    SequenceQuantity = 1
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line2,
                    Point = absPoint6,
                    Index = 2,
                    SequenceQuantity = 2
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line2,
                    Point = absPoint7,
                    Index = 3,
                    SequenceQuantity = 3
                },
                new LinePoint(Guid.NewGuid(), NextCreatedTime())
                {
                    Line = line2,
                    Point = absPoint4,
                    Index = 4,
                    SequenceQuantity = 4
                }
            };

            var ellipse1 = new Ellipse(Guid.NewGuid(), NextCreatedTime())
            {
                CentrePoint = absPoint1,
                FirstConjugateDiameterPoint = absPoint2,
                SecondConjugateDiameterPoint = absPoint3
            };

            var corridorArea1 = new CorridorArea(Guid.NewGuid(), NextCreatedTime())
            {
                CenterLine = line1,
                WidthDimension = 1.5
            };

            var polygonArea1 = new PolygonArea(Guid.NewGuid(), NextCreatedTime())
            {
                BoundingLine = line2,
            };

            var fanArea1 = new FanArea(Guid.NewGuid(), NextCreatedTime())
            {
                VertexPoint = absPoint1,
                MinimumRangeDimension = 1,
                MaximumRangeDimension = 3,
                OrientationAngle = 30,
                SectorSizeAngle = 60
            };

            var pointReference1 = new PointReference(Guid.NewGuid(), NextCreatedTime())
            {
                OriginPoint = absPoint1,
                XVectorPoint = absPoint2,
                YVectorPoint = absPoint3
            };

            var pointReference2 = new PointReference(Guid.NewGuid(), NextCreatedTime())
            {
                OriginPoint = absPoint2,
                XVectorPoint = absPoint3,
                YVectorPoint = absPoint4
            };

            var pointReferences = new List<PointReference>
            {
                pointReference1, pointReference2
            };

            context.Add(verticalDistance1);
            context.AddRange(absolutePoints);
            context.AddRange(lines);
            context.AddRange(linePoints);
            context.Add(ellipse1);
            context.Add(corridorArea1);
            context.Add(polygonArea1);
            context.Add(fanArea1);
            context.AddRange(pointReferences);
            context.SaveChanges();
        }

        private static void SeedObservingFacilities(
            DbContext context)
        {
            var observingFacility1 = new ObservingFacility(Guid.NewGuid(), NextCreatedTime())
            {
                Name = "Livgardens Kaserne"
            };

            var observingFacility2 = new ObservingFacility(Guid.NewGuid(), NextCreatedTime())
            {
                Name = "Uggerby"
            };

            var observingFacilities = new List<ObservingFacility>
            {
                observingFacility1,
                observingFacility2
            };

            context.AddRange(observingFacilities);
            context.SaveChanges();
        }

        private static DateTime NextCreatedTime()
        {
            return _now + new TimeSpan(_delayInTicks++);
        }
    }
}
