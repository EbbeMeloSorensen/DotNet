using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Craft.Logging;
using PR.Domain;
using PR.IO;
using PR.Persistence;
using Person = PR.Domain.Entities.Person;
using PersonAssociation = PR.Domain.Entities.PersonAssociation;

namespace PR.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
        protected ILogger _logger;
        private readonly IDataIOHandler _dataIOHandler;

        public abstract IUnitOfWorkFactory UnitOfWorkFactory { get; }

        protected UIDataProviderBase(
            IDataIOHandler dataIOHandler)
        {
            _dataIOHandler = dataIOHandler;
        }

        public void ExportData(
            string fileName,
            IList<Expression<Func<Person, bool>>> predicates)
        {
            _logger?.WriteLine(LogMessageCategory.Information, $"Exporting data..");

            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            IList<Person> people;
            IList<PersonAssociation> personAssociations;

            if (predicates == null || predicates.Count == 0)
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all person records from repository..", "general", true);

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    people = unitOfWork.People.GetAll().ToList();
                }

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    personAssociations = unitOfWork.PersonAssociations.GetAll()
                        .Select(pa => pa.Clone())
                        .ToList();
                }
            }
            else
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving matching person records from repository..", "general", true);

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    people = unitOfWork.People.Find(predicates).ToList();
                }

                // Todo: Handle person associtations
                throw new NotImplementedException();
            }

            _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {people.Count} person records");

            var numberOfDistinctCreateTimesForPeople = people.Select(p => p.Created).Distinct().Count();
            var numberOfDistinctCreateTimesForPeopleAssociations = personAssociations.Select(p => p.Created).Distinct().Count();

            if (numberOfDistinctCreateTimesForPeople < people.Count ||
                numberOfDistinctCreateTimesForPeopleAssociations < personAssociations.Count)
            {
                // We want to be able to sort items by created date, so we can easily inspect diffs for subsequent versions of the dataset.
                // For this application, records are practically generated one at a time, so the presence of different rows with the same
                // creation time would reflect that the database was populated with data that violates this principle
                throw new InvalidDataException("created times not distinct");
            }

            _logger?.WriteLine(LogMessageCategory.Information, $"  Ordering records..", "general", true);

            var prData = new PRData
            {
                People = people.OrderBy(p => p.Created).ToList(),
                PersonAssociations = personAssociations.OrderBy(pa => pa.Created).ToList()
            };

            _logger?.WriteLine(LogMessageCategory.Information, $"  Done..");

            switch (extension)
            {
                case ".xml":
                    {
                        _logger?.WriteLine(LogMessageCategory.Information, $"  Exporting as xml..", "general", true);
                        _dataIOHandler.ExportDataToXML(prData, fileName);
                        _logger?.WriteLine(LogMessageCategory.Information,
                            $"  Exported {people.Count} person records and {personAssociations.Count} person association records to xml file");
                        break;
                    }
                case ".json":
                    {
                        _logger?.WriteLine(LogMessageCategory.Information, $"  Exporting as json..", "general", true);
                        _dataIOHandler.ExportDataToJson(prData, fileName);
                        _logger?.WriteLine(LogMessageCategory.Information,
                            $"  Exported {people.Count} person records and {personAssociations.Count} person association records to json file");
                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }
    }
}
