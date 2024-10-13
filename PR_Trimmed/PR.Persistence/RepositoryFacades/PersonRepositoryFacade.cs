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

        private UnitOfWorkFacade _unitOfWorkFacade;

        private IUnitOfWork UnitOfWork => _unitOfWorkFacade.UnitOfWork;
        private DateTime? DatabaseTime => _unitOfWorkFacade.DatabaseTime;

        private DateTime CurrentTime => _unitOfWorkFacade.TransactionTime;

        public PersonRepositoryFacade(
            UnitOfWorkFacade unitOfWorkFacade)
        {
            _unitOfWorkFacade = unitOfWorkFacade;
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

            AddVersionPredicates(predicates, DatabaseTime);
            
            return UnitOfWork.People.Count(predicates);
        }

        public int Count(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, DatabaseTime);

            return UnitOfWork.People.Count(predicates);
        }

        public void Add(
            Person person)
        {
            person.ObjectId = Guid.NewGuid();
            person.Created = DateTime.UtcNow;
            person.Superseded = _maxDate;

            UnitOfWork.People.Add(person);
        }

        public Person Get(
            Guid objectId)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.ObjectId == objectId
            };

            AddVersionPredicates(predicates, DatabaseTime);

            var people = UnitOfWork.People.Find(predicates);
            var person = people.SingleOrDefault();

            if (person == null)
            {
                throw new InvalidOperationException("Tried retrieving person that did not exist at the given time");
            }

            return person;
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

            AddVersionPredicates(predicatesForAccociationsWherePersonIsSubject,DatabaseTime);
            AddVersionPredicates(predicatesForAccociationsWherePersonIsObject, DatabaseTime);

            var personAssociationsWherePersonIsSubject =
                UnitOfWork.PersonAssociations
                    .Find(predicatesForAccociationsWherePersonIsSubject)
                    .ToList();

            var personAssociationsWherePersonIsObject =
                UnitOfWork.PersonAssociations
                    .Find(predicatesForAccociationsWherePersonIsObject)
                    .ToList();

            // Now you have all the relevant person association objects
            // ..Then we want to retrieve all the people that are objects in those associations
            // Det er spild af processorkraft, hvis ikke der er nogen associationer

            var objectIdsOfObjectPeople = personAssociationsWherePersonIsSubject
                .Select(_ => _.ObjectPersonObjectId)
                .ToList();

            var objectIdsOfSubjectPeople = personAssociationsWherePersonIsObject
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

            AddVersionPredicates(predicatesForObjectPeople, DatabaseTime);
            AddVersionPredicates(predicatesForSubjectPeople, DatabaseTime);

            var objectPeopleMap = UnitOfWork.People
                .Find(predicatesForObjectPeople)
                .ToDictionary(p => p.ObjectId, p => p);

            var subjectPeopleMap = UnitOfWork.People
                .Find(predicatesForSubjectPeople)
                .ToDictionary(p => p.ObjectId, p => p);

            // Styk det hele sammen
            personAssociationsWherePersonIsSubject.ForEach(pa =>
            {
                pa.SubjectPerson = person;
                pa.ObjectPerson = objectPeopleMap[pa.ObjectPersonObjectId];
            });

            personAssociationsWherePersonIsObject.ForEach(pa =>
            {
                pa.SubjectPerson = subjectPeopleMap[pa.SubjectPersonObjectId];
                pa.ObjectPerson = person;
            });

            person.ObjectPeople = personAssociationsWherePersonIsSubject;
            person.SubjectPeople = personAssociationsWherePersonIsObject;

            return person;
        }

        public IEnumerable<Person> GetAll()
        {
            var predicates = new List<Expression<Func<Person, bool>>>();

            AddVersionPredicates(predicates, DatabaseTime);

            return UnitOfWork.People.Find(predicates);
        }

        public IEnumerable<Person> Find(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            return Find(predicates);
        }

        public IEnumerable<Person> Find(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, DatabaseTime);

            return UnitOfWork.People.Find(predicates);
        }

        public IList<Person> FindIncludingPersonAssociations(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            return FindIncludingPersonAssociations(predicates);
        }

        public IList<Person> FindIncludingPersonAssociations(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, DatabaseTime);

            var people = Find(predicates).ToList();
            var objectIds = people.Select(p => p.ObjectId).ToList();

            var predicatesForAccociationsWherePeopleAreSubjects = new List<Expression<Func<PersonAssociation, bool>>>
            {
                pa => objectIds.Contains(pa.SubjectPersonObjectId)
            };

            var predicatesForAccociationsWherePeopleAreObjects = new List<Expression<Func<PersonAssociation, bool>>>
            {
                pa => objectIds.Contains(pa.ObjectPersonObjectId)
            };

            AddVersionPredicates(predicatesForAccociationsWherePeopleAreSubjects, DatabaseTime);
            AddVersionPredicates(predicatesForAccociationsWherePeopleAreObjects, DatabaseTime);

            var personAssociationsWherePeopleAreSubjects =
                UnitOfWork.PersonAssociations
                    .Find(predicatesForAccociationsWherePeopleAreSubjects)
                    .ToList();

            var personAssociationsWherePeopleAreObjects =
                UnitOfWork.PersonAssociations
                    .Find(predicatesForAccociationsWherePeopleAreObjects)
                    .ToList();

            // Hvis der ikke blev fundet nogen associations, så er det nedenfor spild af processorkraft

            var objectIdsOfObjectPeople = personAssociationsWherePeopleAreSubjects
                .Select(_ => _.ObjectPersonObjectId)
                .ToList();

            var objectIdsOfSubjectPeople = personAssociationsWherePeopleAreObjects
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

            AddVersionPredicates(predicatesForObjectPeople, DatabaseTime);
            AddVersionPredicates(predicatesForSubjectPeople, DatabaseTime);

            var objectPeopleMap = UnitOfWork.People
                .Find(predicatesForObjectPeople)
                .ToDictionary(p => p.ObjectId, p => p);

            var subjectPeopleMap = UnitOfWork.People
                .Find(predicatesForSubjectPeople)
                .ToDictionary(p => p.ObjectId, p => p);

            var peopleMap = people
                .ToDictionary(p => p.ObjectId, p => p);

            // Styk det hele sammen
            personAssociationsWherePeopleAreSubjects.ForEach(pa =>
            {
                pa.SubjectPerson = peopleMap[pa.SubjectPersonObjectId];
                pa.ObjectPerson = objectPeopleMap[pa.ObjectPersonObjectId];
            });

            personAssociationsWherePeopleAreObjects.ForEach(pa =>
            {
                pa.SubjectPerson = subjectPeopleMap[pa.SubjectPersonObjectId];
                pa.ObjectPerson = peopleMap[pa.ObjectPersonObjectId];
            });

            personAssociationsWherePeopleAreSubjects
                .GroupBy(pa => pa.SubjectPersonObjectId)
                .ToList()
                .ForEach(pag =>
                {
                    var objectId = pag.Key;
                    peopleMap[objectId].ObjectPeople = pag.ToList();
                });

            personAssociationsWherePeopleAreObjects
                .GroupBy(pa => pa.ObjectPersonObjectId)
                .ToList()
                .ForEach(pag =>
                {
                    var objectId = pag.Key;
                    peopleMap[objectId].SubjectPeople = pag.ToList();
                });

            people.ForEach(p =>
            {
                p.SubjectPeople ??= new List<PersonAssociation>();
                p.ObjectPeople ??= new List<PersonAssociation>();
            });

            return people;
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

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);

            var newObjects = people.Select(p =>
            {
                var newObject = p.Clone();
                newObject.Id = Guid.Empty;
                newObject.Created = CurrentTime;
                newObject.Superseded = _maxDate;

                return newObject;
            });

            UnitOfWork.People.AddRange(newObjects);
        }

        public void Remove(
            Person person)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(
            IEnumerable<Person> people)
        {
            var objectIds = people.Select(p => p.ObjectId).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => objectIds.Contains(p.ObjectId)
            };

            var objectsFromRepository = Find(predicates).ToList();

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);
        }

        private void AddVersionPredicates(
            ICollection<Expression<Func<PersonAssociation, bool>>> predicates,
            DateTime? databaseTime)
        {
            if (databaseTime.HasValue)
            {
                predicates.Add(pa =>
                    pa.Created <= DatabaseTime &&
                    pa.Superseded > DatabaseTime);
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
                    pa.Created <= DatabaseTime &&
                    pa.Superseded > DatabaseTime);
            }
            else
            {
                predicates.Add(pa =>
                    pa.Superseded.Year == 9999);
            }
        }
    }
}
