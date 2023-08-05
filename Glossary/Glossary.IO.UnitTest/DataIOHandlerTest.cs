using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
                .ExportDataToXML(GenerateDataSet(), "Temp.xml");
        }

        [Fact]
        public void ImportDataFromXML_Works()
        {
            new DataIOHandler().ImportDataFromXML(@"Data/Glossary.xml", out GlossaryData glossaryData);

            glossaryData.Records.Count.Should().Be(3);
            glossaryData.Records.Count(p => p.Term == "Kafka").Should().Be(1);
            glossaryData.Records.Count(p => p.Term == "Javascript").Should().Be(1);
        }

        [Fact]
        public void ExportDataToJson_Works()
        {
            new DataIOHandler().ExportDataToJson(GenerateDataSet(), "Temp.json");
        }

        [Fact]
        public void ImportDataFromJson_Works()
        {
            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ImportDataFromJson(@"Data/Glossary.json", out var glossaryData);

            glossaryData.Records.Count.Should().Be(3);
            glossaryData.Records.Count(p => p.Term == "Kafka").Should().Be(1);
            glossaryData.Records.Count(p => p.Term == "Javascript").Should().Be(1);
        }

        // Helper
        private GlossaryData GenerateDataSet()
        {
            var now = DateTime.UtcNow;

            var kafka = new Record
            {
                Id = Guid.NewGuid(),
                Term = "Kafka",
                Source = "Udemy",
                Category = "Programming",
                Description = "Message Switch",
                Created = new DateTime(2022, 1, 1, 3, 3, 6).ToUniversalTime()
            };

            var python = new Record
            {
                Id = Guid.NewGuid(),
                Term = "Python",
                Created = now
            };

            var javascript = new Record
            {
                Id = Guid.NewGuid(),
                Term = "Javascript",
                Created = now
            };

            return new GlossaryData
            {
                Records = new List<Record>
                {
                    kafka,
                    python,
                    javascript
                },
                RecordAssociations = new List<RecordAssociation>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "is related to",
                        Created = now,
                        ObjectRecordId = kafka.Id,
                        SubjectRecordId = javascript.Id
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "is related to",
                        Created = now,
                        ObjectRecordId = python.Id,
                        SubjectRecordId = kafka.Id
                    }
                }
            };
        }
    }
}