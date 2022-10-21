using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Glossary.Domain.Foreign;
using Xunit;
using Record = Glossary.Domain.Entities.Record;
using RecordAssociation = Glossary.Domain.Entities.RecordAssociation;

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
            new DataIOHandler().ImportDataFromXML(@"C:/Temp/People.xml", out GlossaryData prData);

            prData.People.Count.Should().Be(3);
            prData.People.Count(p => p.Term == "Ebbe").Should().Be(1);
            prData.People.Count(p => p.Term == "Uffe").Should().Be(1);
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
            prData.People.Count(p => p.Term == "Ebbe").Should().Be(1);
            prData.People.Count(p => p.Term == "Uffe").Should().Be(1);
        }

        [Fact]
        public void ImportForeignDataFromJson_Works()
        {
            var dataIOHandler = new DataIOHandler();
            ContactData contactData;
            dataIOHandler.ImportForeignDataFromJson(@"C:/Temp/Contacts.json", out contactData);
        }

        // Helper
        private GlossaryData GenerateDataSet()
        {
            var now = DateTime.UtcNow;

            var ebbe = new Record
            {
                Id = Guid.NewGuid(),
                Term = "Ebbe",
                Source = "Danshøjvej 33",
                Category = "Familie",
                Description = "Mig selv",
                Created = new DateTime(2022, 1, 1, 3, 3, 6).ToUniversalTime()
            };

            var ana = new Record
            {
                Id = Guid.NewGuid(),
                Term = "Ana Tayze",
                Created = now
            };

            var uffe = new Record
            {
                Id = Guid.NewGuid(),
                Term = "Uffe",
                Created = now
            };

            return new GlossaryData
            {
                People = new List<Record>
                {
                    ebbe,
                    ana,
                    uffe
                },
                RecordAssociations = new List<RecordAssociation>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "is the brother of",
                        Created = now,
                        ObjectRecordId = ebbe.Id,
                        SubjectRecordId = uffe.Id
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "is married with",
                        Created = now,
                        ObjectRecordId = ana.Id,
                        SubjectRecordId = ebbe.Id
                    }
                }
            };
        }
    }
}