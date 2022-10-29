using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain;
using PR.Domain.Foreign;
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

        public abstract void CreatePerson(Person person);

        public abstract void CreatePersonAssociation(
            PersonAssociation personAssociation);

        public abstract int CountPeople(
            Expression<Func<Person, bool>> predicate);

        public abstract Person GetPerson(
            Guid id);

        public abstract Person GetPersonWithAssociations(
            Guid id);

        public abstract IList<Person> GetAllPeople();

        public abstract IList<PersonAssociation> GetAllPersonAssociations();

        public abstract IList<Person> FindPeople(
            Expression<Func<Person, bool>> predicate);

        public abstract IList<Person> FindPeople(
            IList<Expression<Func<Person, bool>>> predicates);

        public abstract void UpdatePerson(Person person);

        public abstract void UpdatePeople(IList<Person> people);

        public abstract void UpdatePersonAssociation(PersonAssociation personAssociation);

        public abstract void DeletePerson(Person person);

        public abstract void DeletePeople(IList<Person> people);

        public abstract void DeletePersonAssociations(
            IList<PersonAssociation> personAssociations);

        public void ExportData(
            string fileName,
            IList<Expression<Func<Person, bool>>> predicates)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            IList<Person> people;
            IList<PersonAssociation> personAssociations;

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

            // Temporary: Fix the created dates, so there are no duplets (we want to sort by created date,
            // so we can easily inspect diffs for subsequent versions of the dataset)
            /*
            var personMap = new Dictionary<DateTime, List<Person>>();
            foreach (var person in people)
            {
                if (!personMap.ContainsKey(person.Created))
                {
                    personMap[person.Created] = new List<Person>();
                }

                personMap[person.Created].Add(person);
            }

            foreach (var kvp in personMap)
            {
                if (kvp.Value.Count == 1) continue;

                var temp = kvp.Value
                    .OrderBy(p => p.FirstName)
                    .ThenBy(p => p.Surname)
                    .ToList();

                for (var index = 0; index < kvp.Value.Count; index++)
                {
                    temp[index].Created = temp[index].Created.AddMilliseconds(index);
                }
            }

            var personAssociationMap = new Dictionary<DateTime, List<PersonAssociation>>();
            foreach (var personAssociation in personAssociations)
            {
                if (!personAssociationMap.ContainsKey(personAssociation.Created))
                {
                    personAssociationMap[personAssociation.Created] = new List<PersonAssociation>();
                }

                personAssociationMap[personAssociation.Created].Add(personAssociation);
            }

            foreach (var kvp in personAssociationMap)
            {
                if (kvp.Value.Count == 1) continue;

                for (var index = 0; index < kvp.Value.Count; index++)
                {
                    kvp.Value[index].Created = kvp.Value[index].Created.AddMilliseconds(index).AddSeconds(1);
                }
            }
            */

            var numberOfDistinctCreateTimesForPeople = people.Select(p => p.Created).Distinct().Count();
            var numberOfDistinctCreateTimesForPeopleAssociations = personAssociations.Select(p => p.Created).Distinct().Count();

            if (numberOfDistinctCreateTimesForPeople < people.Count ||
                numberOfDistinctCreateTimesForPeopleAssociations < personAssociations.Count)
            {
                throw new InvalidOperationException("created times not distinct");
            }

            var prData = new PRData
            {
                People = people.OrderBy(p => p.Created).ToList(),
                PersonAssociations = personAssociations.OrderBy(pa => pa.Created).ToList()
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

            var prData = new PRData();

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

                        prData.People = new List<Person>();
                        var personIdMap = new Dictionary<int, Guid>();

                        contactData.People.ForEach(p =>
                        {
                            var person = p.ConvertFromLegacyPerson();
                            personIdMap[p.Id] = person.Id;
                            prData.People.Add(person);
                        });

                        prData.PersonAssociations = new List<PersonAssociation>(contactData.PersonAssociations.Select(
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

            LoadPeople(prData.People);
            LoadPersonAssociations(prData.PersonAssociations);
        }

        public event EventHandler<PersonEventArgs> PersonCreated;
        public event EventHandler<PeopleEventArgs> PeopleUpdated;
        public event EventHandler<PeopleEventArgs> PeopleDeleted;

        protected virtual void OnPersonCreated(
            Person person)
        {
            var handler = PersonCreated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new PersonEventArgs(person));
            }
        }

        protected virtual void OnPeopleUpdated(
            IEnumerable<Person> people)
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
            IEnumerable<Person> people)
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
            IList<Person> people);

        protected abstract void LoadPersonAssociations(
            IList<PersonAssociation> personAssociations);
    }
}
