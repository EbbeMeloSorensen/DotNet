using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PR.Domain;
using PR.Domain.Entities;

namespace PR.Persistence.RepositoryFacades
{
    public class PersonRepositoryFacade
    {
        private static DateTime _maxDate;

        static PersonRepositoryFacade()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private IUnitOfWork _unitOfWork;
        private DateTime? _databaseTime;

        public PersonRepositoryFacade(
            UnitOfWorkFacade unitOfWorkFacade)
        {
            _unitOfWork = unitOfWorkFacade.UnitOfWork;
            _databaseTime = unitOfWorkFacade.DatabaseTime;
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            AddVersionPredicates(predicates, _databaseTime);
            
            return _unitOfWork.People.Count(predicates);
        }

        public int Count(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, _databaseTime);

            return _unitOfWork.People.Count(predicates);
        }

        public void Add(
            Person person)
        {
            person.ObjectId = Guid.NewGuid();
            person.Created = DateTime.UtcNow;
            person.Superseded = _maxDate;

            _unitOfWork.People.Add(person);
        }

        public Person Get(
            Guid objectId)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.ObjectId == objectId
            };

            AddVersionPredicates(predicates, _databaseTime);

            var people = _unitOfWork.People.Find(predicates);
            var person = people.SingleOrDefault();

            if (person == null)
            {
                throw new InvalidOperationException("Tried retrieving person that did not exist at the given time");
            }

            return person;
        }

        public IEnumerable<Person> GetAll()
        {
            var predicates = new List<Expression<Func<Person, bool>>>();

            AddVersionPredicates(predicates, _databaseTime);

            return _unitOfWork.People.Find(predicates);
        }

        public IEnumerable<Person> Find(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            AddVersionPredicates(predicates, _databaseTime);

            return _unitOfWork.People.Find(predicates);
        }

        public IEnumerable<Person> Find(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, _databaseTime);

            return _unitOfWork.People.Find(predicates);
        }

        public Person GetIncludingPersonAssociations(
            Guid objectId)
        {
            var person = Get(objectId);

            var predicatesForAccociationsWherePersonIsSubject = new List<Expression<Func<PersonAssociation, bool>>>
            {
                pa => pa.SubjectPersonObjectId == objectId
            };

            var predicatesForAccociationsWherePersonIsObject = new List<Expression<Func<PersonAssociation, bool>>>
            {
                pa => pa.ObjectPersonObjectId == objectId
            };

            AddVersionPredicates(predicatesForAccociationsWherePersonIsSubject, _databaseTime);
            AddVersionPredicates(predicatesForAccociationsWherePersonIsObject, _databaseTime);

            var personAssociationsWherePersonOfInterestIsSubject =
                _unitOfWork.PersonAssociations
                    .Find(predicatesForAccociationsWherePersonIsSubject)
                    .ToList();

            var personAssociationsWherePersonOfInterestIsObject=
                _unitOfWork.PersonAssociations
                    .Find(predicatesForAccociationsWherePersonIsObject)
                    .ToList();

            // Now you have all the relevant person association objects
            // ..Then we want to retrieve all the people that are objects in those associations

            var objectIdsOfObjectPeople = personAssociationsWherePersonOfInterestIsSubject
                .Select(_ => _.ObjectPersonObjectId)
                .ToList();

            var objectIdsOfSubjectPeople = personAssociationsWherePersonOfInterestIsObject
                .Select(_ => _.SubjectPersonObjectId)
                .ToList();

            var predicatesForObjectPeople = new List<Expression<Func<Person, bool>>>
            {
                p => objectIdsOfObjectPeople.Contains(p.ObjectId)
            };

            var predicatesForSubjectPeople = new List<Expression<Func<Person, bool>>>
            {
                p => objectIdsOfSubjectPeople.Contains(p.ObjectId)
            };

            AddVersionPredicates(predicatesForObjectPeople, _databaseTime);
            AddVersionPredicates(predicatesForSubjectPeople, _databaseTime);

            var objectPeopleMap = _unitOfWork.People
                .Find(predicatesForObjectPeople)
                .ToDictionary(p => p.ObjectId, p => p);

            var subjectPeopleMap = _unitOfWork.People
                .Find(predicatesForSubjectPeople)
                .ToDictionary(p => p.ObjectId, p => p);

            // Styk det hele sammen
            personAssociationsWherePersonOfInterestIsSubject.ForEach(pa =>
            {
                pa.SubjectPerson = person;
                pa.ObjectPerson = objectPeopleMap[pa.ObjectPersonObjectId];
            });

            personAssociationsWherePersonOfInterestIsObject.ForEach(pa =>
            {
                pa.SubjectPerson = subjectPeopleMap[pa.SubjectPersonObjectId];
                pa.ObjectPerson = person;
            });

            person.ObjectPeople = personAssociationsWherePersonOfInterestIsSubject;
            person.SubjectPeople = personAssociationsWherePersonOfInterestIsObject;

            return person;
        }

        public void UpdateRange(
            IEnumerable<Person> people)
        {
            var objectIds = people.Select(p => p.ObjectId).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => objectIds.Contains(p.ObjectId)
            };

            var objectsFromRepository = Find(predicates).ToList();

            var currentTime = DateTime.UtcNow;

            objectsFromRepository.ForEach(p => p.Superseded = currentTime);

            var newObjects = people.Select(p =>
            {
                var newObject = p.Clone();
                newObject.Id = Guid.Empty;
                newObject.Created = currentTime;
                newObject.Superseded = _maxDate;

                return newObject;
            });

            _unitOfWork.People.AddRange(newObjects);
        }

        private void AddVersionPredicates(
            ICollection<Expression<Func<PersonAssociation, bool>>> predicates,
            DateTime? databaseTime)
        {
            if (databaseTime.HasValue)
            {
                predicates.Add(pa =>
                    pa.Created <= _databaseTime &&
                    pa.Superseded > _databaseTime);
            }
            else
            {
                predicates.Add(pa =>
                    pa.Superseded.Year == 9999);
            }
        }

        private void AddVersionPredicates(
            ICollection<Expression<Func<Person, bool>>> predicates,
            DateTime? databaseTime)
        {
            if (databaseTime.HasValue)
            {
                predicates.Add(pa =>
                    pa.Created <= _databaseTime &&
                    pa.Superseded > _databaseTime);
            }
            else
            {
                predicates.Add(pa =>
                    pa.Superseded.Year == 9999);
            }
        }
    }
}
