using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using PR.Domain;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.Versioned.Repositories
{
    public class PersonRepositoryFacade : IPersonRepository
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
            person.Id = Guid.NewGuid();
            person.Created = DateTime.UtcNow;
            person.Superseded = _maxDate;

            UnitOfWork.People.Add(person);
        }

        public void AddRange(
            IEnumerable<Person> person)
        {
            throw new NotImplementedException();
        }

        public Person Get(
            Guid id)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.Id == id
            };

            AddVersionPredicates(predicates, DatabaseTime);

            var people = UnitOfWork.People.Find(predicates);
            var person = people.SingleOrDefault();

            if (person == null)
            {
                throw new InvalidOperationException("Person does not exist");
            }

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

        public Person SingleOrDefault(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(
            Person person)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(
            IEnumerable<Person> people)
        {
            var ids = people.Select(p => p.Id).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => ids.Contains(p.Id)
            };

            var objectsFromRepository = Find(predicates).ToList();

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);

            var newObjects = people.Select(p =>
            {
                var newObject = p.Clone();
                //newObject.Id = Guid.Empty;
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
            var ids = people.Select(p => p.Id).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => ids.Contains(p.Id)
            };

            var objectsFromRepository = Find(predicates).ToList();

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
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
