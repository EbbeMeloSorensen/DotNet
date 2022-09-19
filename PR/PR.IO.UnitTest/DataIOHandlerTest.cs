using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PR.Domain.Foreign;
using Xunit;
using Person = PR.Domain.Entities.Person;

namespace PR.IO.UnitTest
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
            new DataIOHandler().ImportDataFromXML(@"C:/Temp/People.xml", out IList<Person> people);

            people.Count.Should().Be(3);
            people.Count(p => p.FirstName == "Bamse").Should().Be(1);
            people.Count(p => p.FirstName == "Kylling").Should().Be(1);
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
            IList<Person> people;
            dataIOHandler.ImportDataFromJson(@"C:/Temp/People.json", out people);

            people.Count.Should().Be(3);
            people.Count(p => p.FirstName == "Bamse").Should().Be(1);
            people.Count(p => p.FirstName == "Kylling").Should().Be(1);
        }

        [Fact]
        public void ImportForeignDataFromJson_Works()
        {
            var dataIOHandler = new DataIOHandler();
            ContactData contactData;
            dataIOHandler.ImportForeignDataFromJson(@"C:/Temp/Contacts.json", out contactData);
        }

        // Helper
        private List<Person> GenerateDataSet()
        {
            var now = DateTime.UtcNow;

            return new List<Person>
            {
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Bamse",
                    Surname = "Hansen",
                    Created = now
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Kylling",
                    Created = now
                },
                new Person
                {
                    FirstName = "Ana Tayze",
                    Surname = "Melo Sørensen",
                    Nickname = "Hamos",
                    Address = "Danshøjvej 33",
                    ZipCode = "2500",
                    City = "Valby",
                    Birthday = new DateTime(1980, 6, 13).ToUniversalTime(),
                    Category = "Familie",
                    Description = "Min kone",
                    Dead = false,
                    Created = new DateTime(2022, 1, 1, 3, 3, 6).ToUniversalTime()
                }
            };
        }
    }
}