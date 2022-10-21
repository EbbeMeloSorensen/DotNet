using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using Glossary.Domain;
using Glossary.Domain.Foreign;
using Glossary.IO;
using Glossary.Persistence;
using Record = Glossary.Domain.Entities.Record;
using RecordAssociation = Glossary.Domain.Entities.RecordAssociation;

namespace Glossary.Application
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

        public virtual void Initialize(
            ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task<bool> CheckConnection();
        public int CountAllPeople()
        {
            throw new NotImplementedException();
        }

        public abstract void CreateRecord(Record person);

        public abstract void CreateRecordAssociation(
            RecordAssociation personAssociation);

        public abstract int CountPeople(
            Expression<Func<Record, bool>> predicate);

        public abstract Record GetRecord(
            Guid id);

        public abstract Record GetPersonWithAssociations(
            Guid id);

        public abstract IList<Record> GetAllPeople();

        public abstract IList<RecordAssociation> GetAllPersonAssociations();

        public abstract IList<Record> FindPeople(
            Expression<Func<Record, bool>> predicate);

        public abstract IList<Record> FindPeople(
            IList<Expression<Func<Record, bool>>> predicates);

        public abstract void UpdatePerson(Record person);

        public abstract void UpdatePeople(IList<Record> people);

        public abstract void UpdatePersonAssociation(RecordAssociation personAssociation);

        public abstract void DeletePerson(Record person);

        public abstract void DeletePeople(IList<Record> people);

        public abstract void DeletePersonAssociations(
            IList<RecordAssociation> personAssociations);

        public void ExportData(
            string fileName,
            IList<Expression<Func<Record, bool>>> predicates)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            IList<Record> people;
            IList<RecordAssociation> personAssociations;

            if (predicates == null || predicates.Count == 0)
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all person records from repository..");
                people = GetAllPeople();
                personAssociations = GetAllPersonAssociations();
            }
            else
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving matching person records from repository..");
                people = FindPeople(predicates);

                // Todo: Handle person associtations
                throw new NotImplementedException();
            }

            _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {people.Count} person records");

            var prData = new GlossaryData
            {
                Records = people.ToList(),
                RecordAssociations = personAssociations.ToList()
            };

            switch (extension)
            {
                case ".xml":
                    {
                        _dataIOHandler.ExportDataToXML(prData, fileName);
                        _logger?.WriteLine(LogMessageCategory.Information,
                            $"  Exported {people.Count} person records to xml file");
                        break;
                    }
                case ".json":
                    {
                        _dataIOHandler.ExportDataToJson(prData, fileName);
                        _logger?.WriteLine(LogMessageCategory.Information,
                            $"  Exported {people.Count} person records to json file");
                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }

        public void ImportData(
            string fileName,
            bool legacy)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            var prData = new GlossaryData();

            switch (extension)
            {
                case ".xml":
                {
                    _dataIOHandler.ImportDataFromXML(
                        fileName, out prData);
                    break;
                }
                case ".json":
                {
                    if (legacy)
                    {
                        _dataIOHandler.ImportForeignDataFromJson(fileName, out var contactData);

                        prData.Records = new List<Record>();
                        var personIdMap = new Dictionary<int, Guid>();

                        contactData.People.ForEach(p =>
                        {
                            var person = p.ConvertFromLegacyPerson();
                            personIdMap[p.Id] = person.Id;
                            prData.Records.Add(person);
                        });

                        prData.RecordAssociations = new List<RecordAssociation>(contactData.PersonAssociations.Select(
                            pa => pa.ConvertFromLegacyPersonAssociation(personIdMap)));
                    }
                    else
                    {
                        _dataIOHandler.ImportDataFromJson(
                            fileName, out prData);
                    }
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }

            LoadPeople(prData.Records);
            LoadPersonAssociations(prData.RecordAssociations);
        }

        public event EventHandler<PersonEventArgs> PersonCreated;
        public event EventHandler<PeopleEventArgs> PeopleUpdated;
        public event EventHandler<PeopleEventArgs> PeopleDeleted;

        protected virtual void OnPersonCreated(
            Record person)
        {
            var handler = PersonCreated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new PersonEventArgs(person));
            }
        }

        protected virtual void OnPeopleUpdated(
            IEnumerable<Record> people)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = PeopleUpdated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new PeopleEventArgs(people));
            }
        }

        protected virtual void OnPeopleDeleted(
            IEnumerable<Record> people)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = PeopleDeleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new PeopleEventArgs(people));
            }
        }

        protected abstract void LoadPeople(
            IList<Record> people);

        protected abstract void LoadPersonAssociations(
            IList<RecordAssociation> personAssociations);
    }
}
