using System;
using System.Collections.Generic;
using System.Linq;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

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

        public PersonRepositoryFacade(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            Guid objectId,
            DateTime? databaseTime = null)
        {
            IEnumerable<Person> people;

            if (databaseTime.HasValue)
            {
                people = _unitOfWork.People.Find(p => p.ObjectId == objectId &&
                                                    p.Created <= databaseTime &&
                                                    p.Superseded > databaseTime);
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

        public IEnumerable<Person> GetAll(
            DateTime? databaseTime = null)
        {
            if (databaseTime.HasValue)
            {
                return _unitOfWork.People.Find(p => p.Created <= databaseTime &&
                                                    p.Superseded > databaseTime);
            }

            return _unitOfWork.People.Find(p => p.Superseded.Year == 9999);
        }

        public Person GetIncludingPersonAssociations(
            Guid objectId,
            DateTime? databaseTime = null)
        {
            var person = Get(objectId, databaseTime);
            IEnumerable<PersonAssociation> personAssociationsWherePersonOfInterestIsSubject;
            IEnumerable<PersonAssociation> personAssociationsWherePersonOfInterestIsObject;

            if (databaseTime.HasValue)
            {
                personAssociationsWherePersonOfInterestIsSubject = _unitOfWork.PersonAssociations.Find(
                    pa => pa.SubjectPersonObjectId == objectId &&
                          pa.Created <= databaseTime &&
                          pa.Superseded > databaseTime);

                personAssociationsWherePersonOfInterestIsObject = _unitOfWork.PersonAssociations.Find(
                    pa => pa.ObjectPersonObjectId == objectId &&
                          pa.Created <= databaseTime &&
                          pa.Superseded > databaseTime);
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
