﻿using System;
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
using Person = Glossary.Domain.Entities.Person;
using PersonAssociation = Glossary.Domain.Entities.PersonAssociation;

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

            var prData = new PRData
            {
                People = people.ToList(),
                PersonAssociations = personAssociations.ToList()
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