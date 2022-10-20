using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Glossary.Domain.Foreign;
using Xunit;
using Person = Glossary.Domain.Entities.Person;
using PersonAssociation = Glossary.Domain.Entities.PersonAssociation;

namespace Glossary.IO.UnitTest
{
    public class DataIOHandlerTest
    {
        [Fact]
        public void ExportDataToXML_Works()
        {
            new DataIOHandler()
                .ExportDataToXML(GenerateDataSet(), @"C:/Temp/People.xml");
        }

        [Fact]
        public void ImportDataFromXML_Works()
        {
            new DataIOHandler().ImportDataFromXML(@"C:/Temp/People.xml", out PRData prData);

            prData.People.Count.Should().Be(3);
            prData.People.Count(p => p.FirstName == "Ebbe").Should().Be(1);
            prData.People.Count(p => p.FirstName == "Uffe").Should().Be(1);
        }

        [Fact]
        public void ExportDataToJson_Works()
        {
            new DataIOHandler().ExportDataToJson(GenerateDataSet(), @"C:/Temp/People.json");
        }

        [Fact]
        public void ImportDataFromJson_Works()
        {
            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ImportDataFromJson(@"C:/Temp/People.json", out var prData);

            prData.People.Count.Should().Be(3);
            prData.People.Count(p => p.FirstName == "Ebbe").Should().Be(1);
            prData.People.Count(p => p.FirstName == "Uffe").Should().Be(1);
        }

        [Fact]
        public void ImportForeignDataFromJson_Works()
        {
            var dataIOHandler = new DataIOHandler();
            ContactData contactData;
            dataIOHandler.ImportForeignDataFromJson(@"C:/Temp/Contacts.json", out contactData);
        }

        // Helper
        private PRData GenerateDataSet()
        {
            var now = DateTime.UtcNow;

            var ebbe = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Ebbe",
                Address = "Danshøjvej 33",
                Category = "Familie",
                Description = "Mig selv",
                Created = new DateTime(2022, 1, 1, 3, 3, 6).ToUniversalTime()
            };

            var ana = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Ana Tayze",
                Created = now
            };

            var uffe = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Uffe",
                Created = now
            };

            return new PRData
            {
                People = new List<Person>
                {
                    ebbe,
                    ana,
                    uffe
                },
                PersonAssociations = new List<PersonAssociation>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "is the brother of",
                        Created = now,
                        ObjectPersonId = ebbe.Id,
                        SubjectPersonId = uffe.Id
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "is married with",
                        Created = now,
                        ObjectPersonId = ana.Id,
                        SubjectPersonId = ebbe.Id
                    }
                }
            };
        }
    }
}