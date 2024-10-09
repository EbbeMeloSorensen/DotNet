using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            if (_databaseTime.HasValue)
            {
                predicates.Add(p =>
                    p.Created < _databaseTime &&
                    p.Superseded > _databaseTime);
            }
            else
            {
                predicates.Add(p =>
                    p.Superseded.Year == 9999);
            }

            return _unitOfWork.People.Count(predicates);
        }

        public int Count(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            if (_databaseTime.HasValue)
            {
                predicates.Add(p =>
                    p.Created < _databaseTime &&
                    p.Superseded > _databaseTime);
            }
            else
            {
                predicates.Add(p =>
                    p.Superseded.Year == 9999);
            }

            return _unitOfWork.People.Count(predicates);
        }

        public void Add(
            Person person)
        {
            person.ObjectId = Guid.NewGuid();
            person.Created = DateTime.Now;
            person.Superseded = _maxDate;

            _unitOfWork.People.Add(person);
        }

        public Person Get(
            Guid objectId)
        {
            IEnumerable<Person> people;

            if (_databaseTime.HasValue)
            {
                people = _unitOfWork.People.Find(p => p.ObjectId == objectId &&
                                                    p.Created <= _databaseTime &&
                                                    p.Superseded > _databaseTime);
            }
            else
            {
                people = _unitOfWork.People.Find(p => p.ObjectId == objectId &&
                                                      p.Superseded.Year == 9999);
            }

            var result = people.SingleOrDefault();

            if (result == null)
            {
                throw new InvalidOperationException("Tried retrieving person that did not exist at the given time");
            }

            return result;
        }

        public IEnumerable<Person> GetAll()
        {
            if (_databaseTime.HasValue)
            {
                return _unitOfWork.People.Find(p => p.Created <= _databaseTime &&
                                                    p.Superseded > _databaseTime);
            }

            return _unitOfWork.People.Find(p => p.Superseded.Year == 9999);
        }

        public IEnumerable<Person> Find(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            if (_databaseTime.HasValue)
            {
                predicates.Add(p =>
                    p.Created < _databaseTime &&
                    p.Superseded > _databaseTime);
            }
            else
            {
                predicates.Add(p =>
                    p.Superseded.Year == 9999);
            }

            return _unitOfWork.People.Find(predicates);
        }

        public IEnumerable<Person> Find(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Person GetIncludingPersonAssociations(
            Guid objectId)
        {
            var person = Get(objectId);
            IEnumerable<PersonAssociation> personAssociationsWherePersonOfInterestIsSubject;
            IEnumerable<PersonAssociation> personAssociationsWherePersonOfInterestIsObject;

            if (_databaseTime.HasValue)
            {
                personAssociationsWherePersonOfInterestIsSubject = _unitOfWork.PersonAssociations.Find(
                    pa => pa.SubjectPersonObjectId == objectId &&
                          pa.Created <= _databaseTime &&
                          pa.Superseded > _databaseTime);

                personAssociationsWherePersonOfInterestIsObject = _unitOfWork.PersonAssociations.Find(
                    pa => pa.ObjectPersonObjectId == objectId &&
                          pa.Created <= _databaseTime &&
                          pa.Superseded > _databaseTime);
            }
            else
            {
                personAssociationsWherePersonOfInterestIsSubject = _unitOfWork.PersonAssociations.Find(
                    pa => pa.SubjectPersonObjectId == objectId &&
                          pa.Superseded.Year == 9999);

                personAssociationsWherePersonOfInterestIsObject = _unitOfWork.PersonAssociations.Find(
                    pa => pa.ObjectPersonObjectId == objectId &&
                          pa.Superseded.Year == 9999);
            }

            person.ObjectPeople = personAssociationsWherePersonOfInterestIsSubject.ToList();
            person.SubjectPeople = personAssociationsWherePersonOfInterestIsObject.ToList();

            return person;
        }
    }
}
