using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PR.Domain.Entities;
using Xunit;

namespace PR.IO.UnitTest
{
    public class DataIOHandlerTest
    {
        [Fact]
        public void ExportDataToXML_Works()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Bamse"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Kylling"
                }
            };

            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ExportDataToXML(people, @"C:/Temp/People.xml");
        }

        [Fact]
        public void ImportDataFromXML_Works()
        {
            var dataIOHandler = new DataIOHandler();
            IList<Person> people;
            dataIOHandler.ImportDataFromXML(@"C:/Temp/People.xml", out people);

            people.Count.Should().Be(2);
            people.Count(p => p.FirstName == "Bamse").Should().Be(1);
            people.Count(p => p.FirstName == "Kylling").Should().Be(1);
        }

        [Fact]
        public void ExportDataToJson_Works()
        {
            var people = new List<Person>
            {
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Bamse"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Kylling"
                }
            };

            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ExportDataToJson(people, @"C:/Temp/People.json");
        }

        [Fact]
        public void ImportDataFromJson_Works()
        {
            var dataIOHandler = new DataIOHandler();
            IList<Person> people;
            dataIOHandler.ImportDataFromJson(@"C:/Temp/People.json", out people);

            people.Count.Should().Be(2);
            people.Count(p => p.FirstName == "Bamse").Should().Be(1);
            people.Count(p => p.FirstName == "Kylling").Should().Be(1);
        }

    }
}