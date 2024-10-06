using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore
{
    public static class Seeding
    {
        public static void SeedDatabase(
            PRDbContextBase context)
        {
            // Star Wars
            // https://cdn.shopify.com/s/files/1/1835/6621/files/star-wars-family-tree-episde-9.png?v=1578255836

            if (context.People.Any()) return;

            var baseTime = new DateTime(2011, 4, 3, 7, 9, 13, DateTimeKind.Utc);
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            var supersedingTime1 = new DateTime(2014, 5, 20, 3, 6, 9, DateTimeKind.Utc);

            var delay = 0;

            var anakin_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = supersedingTime1,
                FirstName = "Anakin",
                Surname = "Skywalker",
                Nickname = "Ani",
                Address = "Tatooine"
            };

            var luke_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF01"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = maxDate,
                FirstName = "Luke",
                Surname = "Skywalker",
                Category = "Jedi",
                Address = "Tatooine"
            };

            var leia_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF02"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = maxDate,
                FirstName = "Leia",
                Surname = "Organa",
                Description = "Princess",
                Address = "Alderaan"
            };

            var anakin_1 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                Created = supersedingTime1,
                Superseded = maxDate,
                FirstName = "Darth",
                Surname = "Vader",
                Category = "Jedi, Sith",
                Address = "Mustafar"
            };

            //var obiWan = new Person
            //{
            //    FirstName = "Obi-Wan",
            //    Surname = "Kenobi",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var padme = new Person
            //{
            //    FirstName = "Padme",
            //    Surname = "Amidala",
            //    Address = "Naboo",
            //    Description = "Princess",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var benSolo = new Person
            //{
            //    FirstName = "Ben",
            //    Surname = "Solo",
            //    Nickname = "Kylo Ren",
            //    Category = "Jedi/Sith",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var hanSolo = new Person
            //{
            //    FirstName = "Han",
            //    Surname = "Solo",
            //    Description = "Smuggler",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var quiGon = new Person
            //{
            //    FirstName = "Qui-Gon",
            //    Surname = "Jinn",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var countDooku = new Person
            //{
            //    FirstName = "Count Dooku",
            //    Nickname = "Darth Tyranus",
            //    Category = "Sith",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var palpatine = new Person
            //{
            //    FirstName = "Emperor Palpatine",
            //    Nickname = "Darth Sidious",
            //    Category = "Sith",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var rey = new Person
            //{
            //    FirstName = "Rey",
            //    Surname = "Skywalker",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var maul = new Person
            //{
            //    FirstName = "Darth Maul",
            //    Category = "Sith",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var plagueis = new Person
            //{
            //    FirstName = "Darth Plagueis",
            //    Category = "Sith",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var yoda = new Person
            //{
            //    FirstName = "Yoda",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var lando = new Person
            //{
            //    FirstName = "Lando",
            //    Surname = "Calrissian",
            //    Description = "Smuggler",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var chewbacca = new Person
            //{
            //    FirstName = "Chewbacca",
            //    Nickname = "Chewie",
            //    Address = "Kashyyyk",
            //    Description = "Wookie",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var bobaFett = new Person
            //{
            //    FirstName = "Boba",
            //    Surname = "Fett",
            //    Address = "Mandalore",
            //    Description = "Bounty Hunter",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var tarkin = new Person
            //{
            //    FirstName = "Wilhuff",
            //    Surname = "Tarkin",
            //    Nickname = "Grand Moff Tarkin",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var maxRebo = new Person
            //{
            //    FirstName = "Max",
            //    Surname = "Rebo",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var nienNunb = new Person
            //{
            //    FirstName = "Nien",
            //    Surname = "Nunb",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var bibFortuna = new Person
            //{
            //    FirstName = "Bib",
            //    Surname = "Fortuna",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var admiralAckbar = new Person
            //{
            //    FirstName = "Ackbar",
            //    Nickname = "Admiral Ackbar",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var wicket = new Person
            //{
            //    FirstName = "Wicket",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var oola = new Person
            //{
            //    FirstName = "Oola",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var yarnaDalGargan = new Person
            //{
            //    FirstName = "Yarna",
            //    Surname = "d'al Gargan",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var dengar = new Person
            //{
            //    FirstName = "Dengar",
            //    Description = "Bounty hunter",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var bossk = new Person
            //{
            //    FirstName = "Bossk",
            //    Description = "Bounty hunter",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var shmi = new Person
            //{
            //    FirstName = "Shmi",
            //    Surname = "Skywalker",
            //    Address = "Tatooine",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var c3po = new Person
            //{
            //    FirstName = "C-3PO",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var r2d2 = new Person
            //{
            //    FirstName = "R2-D2",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var jarjar = new Person
            //{
            //    FirstName = "Jar Jar",
            //    Surname = "Binks",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var generalGrievous = new Person
            //{
            //    FirstName = "Qymaen",
            //    Surname = " jai Sheelal",
            //    Nickname = "General Grievous",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var maceWindu = new Person
            //{
            //    FirstName = "Mace",
            //    Surname = "Windu",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var kiAdiMundi = new Person
            //{
            //    FirstName = "Ki-Adi-Mundi",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var agenKolar = new Person
            //{
            //    FirstName = "Agen",
            //    Surname = "Kolar",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var saeseeTiin = new Person
            //{
            //    FirstName = "Saesee",
            //    Surname = "Tin",
            //    Category = "Jedi",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var kitFisto = new Person
            //{
            //    FirstName = "Kit",
            //    Category = "Fisto",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var jabba = new Person
            //{
            //    FirstName = "Jabba",
            //    Surname = "Desilijic Tiure",
            //    Nickname = "Jabba the Hutt",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var monMothma = new Person
            //{
            //    FirstName = "Mon",
            //    Surname = "Mothma",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var jynErso = new Person
            //{
            //    FirstName = "Jyn",
            //    Surname = "Erso",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var galenErso = new Person
            //{
            //    FirstName = "Galen",
            //    Surname = "Erso",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var orsonKrennic = new Person
            //{
            //    FirstName = "Orson",
            //    Surname = "Krennic",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var chirrutImwe = new Person
            //{
            //    FirstName = "Chirrut",
            //    Surname = "Imwe",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var bazeMalbus = new Person
            //{
            //    FirstName = "Baze",
            //    Surname = "Malbus",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var k2so = new Person
            //{
            //    FirstName = "K-2SO",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var cassian = new Person
            //{
            //    FirstName = "Cassian",
            //    Surname = "Andor",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var sawGerrera = new Person
            //{
            //    FirstName = "Saw",
            //    Surname = "Gerrera",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var luthen = new Person
            //{
            //    FirstName = "Luthen",
            //    Surname = "Rael",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var maarva = new Person
            //{
            //    FirstName = "Maarva",
            //    Surname = "Andor",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var tobiasBeckett = new Person
            //{
            //    FirstName = "Tobias",
            //    Surname = "Beckett",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var qira = new Person
            //{
            //    FirstName = "Qi´ra",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var l337 = new Person
            //{
            //    FirstName = "L3-37",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var drydenVos = new Person
            //{
            //    FirstName = "Dryden",
            //    Surname = "Vos",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var bb8 = new Person
            //{
            //    FirstName = "BB-8",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var poeDameron = new Person
            //{
            //    FirstName = "Poe",
            //    Surname = "Dameron",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var finn = new Person
            //{
            //    FirstName = "Finn",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var mazKanata = new Person
            //{
            //    FirstName = "Maz",
            //    Surname = "Kanata",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var ahsoka = new Person
            //{
            //    FirstName = "Ahsoka",
            //    Surname = "Tano",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var lorSanTekka = new Person
            //{
            //    FirstName = "Lor San",
            //    Surname = "Tekka",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var roseTico = new Person
            //{
            //    FirstName = "Rose",
            //    Surname = "Tico",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var bailOrgana = new Person
            //{
            //    FirstName = "Bail",
            //    Surname = "Organa",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var dinDjarin = new Person
            //{
            //    FirstName = "Din",
            //    Surname = "Djarin",
            //    Nickname = "Mando",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var kuiil = new Person
            //{
            //    FirstName = "Kuiil",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var grogu = new Person
            //{
            //    FirstName = "Grogu",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var boKatanKryze = new Person
            //{
            //    FirstName = "Bo-Katan",
            //    Surname = "Kryze",
            //    Created = now + new TimeSpan(delay++),
            //};

            //var moffGideon = new Person
            //{
            //    FirstName = "Gideon",
            //    Nickname = "Moff Gideon",
            //    Created = now + new TimeSpan(delay++),
            //};

            //pazVizsla
            //eliaKane
            //caraDune
            //axeWoves
            //ig11 // ig series droid & bounty hunter from Mandalorian
            //ig88B // ig series droid & bounty hunter from Empire Strikes Back
            //jangoFett
            //blackKrrsantan
            //armorer
            //4lom // bounty hunter from Empire Strikes Back
            //cadBane // bounty hunter from Mandalorian

            var people = new List<Person>
            {
                anakin_0,
                luke_0,
                leia_0,
                anakin_1,
                //obiWan,
                //padme,
                //benSolo,
                //hanSolo,
                //quiGon,
                //countDooku,
                //palpatine,
                //rey,
                //maul,
                //plagueis,
                //yoda,
                //lando,
                //chewbacca,
                //bobaFett,
                //tarkin,
                //maxRebo,
                //nienNunb,
                //bibFortuna,
                //admiralAckbar,
                //wicket,
                //oola,
                //yarnaDalGargan,
                //dengar,
                //bossk,
                //shmi,
                //c3po,
                //r2d2,
                //jarjar,
                //generalGrievous,
                //maceWindu,
                //kiAdiMundi,
                //agenKolar,
                //saeseeTiin,
                //kitFisto,
                //jabba,
                //monMothma,
                //jynErso,
                //galenErso,
                //orsonKrennic,
                //chirrutImwe,
                //bazeMalbus,
                //k2so,
                //cassian,
                //luthen,
                //sawGerrera,
                //maarva,
                //tobiasBeckett,
                //qira,
                //l337,
                //drydenVos,
                //bb8,
                //poeDameron,
                //finn,
                //mazKanata,
                //ahsoka,
                //lorSanTekka,
                //roseTico,
                //bailOrgana,
                //dinDjarin,
                //kuiil,
                //grogu,
                //boKatanKryze,
                //moffGideon
            };

            var personAssociation1 = new PersonAssociation
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BB0000000000"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = maxDate,
                SubjectPerson = anakin_0,
                Description = "is a parent of",
                ObjectPerson = luke_0,
            };

            var personAssociation2 = new PersonAssociation
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BB0000000001"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = maxDate,
                SubjectPerson = anakin_0,
                Description = "is a parent of",
                ObjectPerson = leia_0,
            };

            var personAssociations = new List<PersonAssociation>
            {
                personAssociation1,
                personAssociation2,
                //new()
                //{
                //    SubjectPerson = anakin,
                //    Description = "is married with",
                //    ObjectPerson = padme,
                //    Created = now + new TimeSpan(delay),
                //},
                //new()
                //{
                //    SubjectPerson = padme,
                //    Description = "is a parent of",
                //    ObjectPerson = luke,
                //    Created = now + new TimeSpan(delay + 3),
                //},
                //new()
                //{
                //    SubjectPerson = padme,
                //    Description = "is a parent of",
                //    ObjectPerson = leia,
                //    Created = now + new TimeSpan(delay + 4),
                //},
                //new()
                //{
                //    SubjectPerson = leia,
                //    Description = "is married with",
                //    ObjectPerson = hanSolo,
                //    Created = now + new TimeSpan(delay + 5),
                //},
                //new()
                //{
                //    SubjectPerson = leia,
                //    Description = "is a parent of",
                //    ObjectPerson = benSolo,
                //    Created = now + new TimeSpan(delay + 6),
                //},
                //new()
                //{
                //    SubjectPerson = hanSolo,
                //    Description = "is a parent of",
                //    ObjectPerson = benSolo,
                //    Created = now + new TimeSpan(delay + 7),
                //},
                //new()
                //{
                //    SubjectPerson = luke,
                //    Description = "is an apprentice of",
                //    ObjectPerson = obiWan,
                //    Created = now + new TimeSpan(delay + 8),
                //},
                //new()
                //{
                //    SubjectPerson = obiWan,
                //    Description = "is an apprentice of",
                //    ObjectPerson = quiGon,
                //    Created = now + new TimeSpan(delay + 9),
                //},
                //new()
                //{
                //    SubjectPerson = quiGon,
                //    Description = "is an apprentice of",
                //    ObjectPerson = countDooku,
                //    Created = now + new TimeSpan(delay + 10)
                //},
                //new()
                //{
                //    SubjectPerson = countDooku,
                //    Description = "is an apprentice of",
                //    ObjectPerson = palpatine,
                //    Created = now + new TimeSpan(delay + 11)
                //},
                //new()
                //{
                //    SubjectPerson = anakin,
                //    Description = "is an apprentice of",
                //    ObjectPerson = palpatine,
                //    Created = now + new TimeSpan(delay + 12)
                //},
                //new()
                //{
                //    SubjectPerson = palpatine,
                //    Description = "is a grandparent of",
                //    ObjectPerson = rey,
                //    Created = now + new TimeSpan(delay + 13)
                //},
                //new()
                //{
                //    SubjectPerson = rey,
                //    Description = "is an apprentice of",
                //    ObjectPerson = luke,
                //    Created = now + new TimeSpan(delay + 14)
                //},
                //new()
                //{
                //    SubjectPerson = rey,
                //    Description = "is an apprentice of",
                //    ObjectPerson = leia,
                //    Created = now + new TimeSpan(delay + 15)
                //},
                //new()
                //{
                //    SubjectPerson = maul,
                //    Description = "is an apprentice of",
                //    ObjectPerson = palpatine,
                //    Created = now + new TimeSpan(delay + 16)
                //},
                //new()
                //{
                //    SubjectPerson = palpatine,
                //    Description = "is an apprentice of",
                //    ObjectPerson = plagueis,
                //    Created = now + new TimeSpan(delay + 17)
                //},
                //new()
                //{
                //    SubjectPerson = countDooku,
                //    Description = "is an apprentice of",
                //    ObjectPerson = yoda,
                //    Created = now + new TimeSpan(delay + 18)
                //},
                //new()
                //{
                //    SubjectPerson = luke,
                //    Description = "is an apprentice of",
                //    ObjectPerson = yoda,
                //    Created = now + new TimeSpan(delay + 19)
                //},
                //new()
                //{
                //    SubjectPerson = lando,
                //    Description = "is a friend of",
                //    ObjectPerson = hanSolo,
                //    Created = now + new TimeSpan(delay + 20)
                //},
                //new()
                //{
                //    SubjectPerson = chewbacca,
                //    Description = "is a friend of",
                //    ObjectPerson = hanSolo,
                //    Created = now + new TimeSpan(delay + 21)
                //},
                //new()
                //{
                //    SubjectPerson = bobaFett,
                //    Description = "is employed by",
                //    ObjectPerson = anakin,
                //    Created = now + new TimeSpan(delay + 22)
                //},
                //new()
                //{
                //    SubjectPerson = anakin,
                //    Description = "is an apprentice of",
                //    ObjectPerson = obiWan,
                //    Created = now + new TimeSpan(delay + 23)
                //},
                //new()
                //{
                //    SubjectPerson = shmi,
                //    Description = "is a parent of",
                //    ObjectPerson = anakin,
                //    Created = now + new TimeSpan(delay + 24)
                //},
                //new()
                //{
                //    SubjectPerson = maarva,
                //    Description = "is an adoptive parent of",
                //    ObjectPerson = cassian,
                //    Created = now + new TimeSpan(delay + 25)
                //},
                //new()
                //{
                //    SubjectPerson = ahsoka,
                //    Description = "is an apprentice of",
                //    ObjectPerson = anakin,
                //    Created = now + new TimeSpan(delay + 26)
                //},
                //new()
                //{
                //    SubjectPerson = poeDameron,
                //    Description = "is a friend of",
                //    ObjectPerson = rey,
                //    Created = now + new TimeSpan(delay + 27)
                //},
                //new()
                //{
                //    SubjectPerson = finn,
                //    Description = "is a friend of",
                //    ObjectPerson = rey,
                //    Created = now + new TimeSpan(delay + 28)
                //},
                //new()
                //{
                //    SubjectPerson = tarkin,
                //    Description = "is a subordinate of",
                //    ObjectPerson = palpatine,
                //    Created = now + new TimeSpan(delay + 29)
                //},
                //new()
                //{
                //    SubjectPerson = anakin,
                //    Description = "is a subordinate of",
                //    ObjectPerson = tarkin,
                //    Created = now + new TimeSpan(delay + 30)
                //}
            };

            context.AddRange(people);
            context.AddRange(personAssociations);
            context.SaveChanges();
        }
    }
}
