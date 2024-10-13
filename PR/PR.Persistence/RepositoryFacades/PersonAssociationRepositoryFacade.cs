using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PR.Domain;
using PR.Domain.Entities;

namespace PR.Persistence.RepositoryFacades
{
    public class PersonAssociationRepositoryFacade
    {
        private static DateTime _maxDate;

        static PersonAssociationRepositoryFacade()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private IUnitOfWork _unitOfWork;
        private DateTime? _databaseTime;

        public PersonAssociationRepositoryFacade(
            UnitOfWorkFacade unitOfWorkFacade)
        {
            _unitOfWork = unitOfWorkFacade.UnitOfWork;
            _databaseTime = unitOfWorkFacade.DatabaseTime;
        }

        public void Add(
            PersonAssociation personAssociation)
        {
            personAssociation.ObjectId = Guid.NewGuid();
            personAssociation.Created = DateTime.UtcNow;
            personAssociation.Superseded = _maxDate;

            _unitOfWork.PersonAssociations.Add(personAssociation);
        }

        public PersonAssociation Get(
            Guid objectId)
        {
            var predicates = new List<Expression<Func<PersonAssociation, bool>>>
            {
                pa => pa.ObjectId == objectId
            };

            AddVersionPredicates(predicates, _databaseTime);

            var personAssociations = _unitOfWork.PersonAssociations.Find(predicates);
            var personAssociation = personAssociations.SingleOrDefault();

            if (personAssociation == null)
            {
                throw new InvalidOperationException("Tried retrieving person association that did not exist at the given time");
            }

            return personAssociation;
        }

        public IEnumerable<PersonAssociation> Find(
            Expression<Func<PersonAssociation, bool>> predicate)
        {
            var predicates = new List<Expression<Func<PersonAssociation, bool>>>
            {
                predicate
            };

            AddVersionPredicates(predicates, _databaseTime);

            return _unitOfWork.PersonAssociations.Find(predicates);
        }

        public IEnumerable<Person> Find(
            IList<Expression<Func<PersonAssociation, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public void Update(
            PersonAssociation personAssociation)
        {
            var objectFromRepository = Get(personAssociation.ObjectId);
            var newObj = personAssociation.Clone();
            var currentTime = DateTime.UtcNow;

            objectFromRepository.Superseded = currentTime;

            newObj.Id = Guid.Empty;
            newObj.Created = currentTime;
            _unitOfWork.PersonAssociations.Add(newObj);
        }

        public void RemoveRange(
            IEnumerable<PersonAssociation> people)
        {
            var currentTime = DateTime.UtcNow;
            people.ToList().ForEach(p => p.Superseded = currentTime);
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
    }
}