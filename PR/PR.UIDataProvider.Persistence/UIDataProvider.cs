using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain;
using PR.Domain.Entities;
using PR.Persistence;
using PR.IO;
using PR.Application;

namespace PR.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        public override IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
        }

        public override void Initialize(
            ILogger logger)
        {
            base.Initialize(logger);

            UnitOfWorkFactory.Initialize(logger);
        }

        public override async Task<bool> CheckConnection()
        {
            return await UnitOfWorkFactory.CheckRepositoryConnection();
        }

        public override void CreatePerson(
            Person person)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.Add(person);
                unitOfWork.Complete();
            }

            OnPersonCreated(person);
        }

        public override void CreatePersonAssociation(
            PersonAssociation personAssociation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.PersonAssociations.Add(personAssociation);
                unitOfWork.Complete();
            }
        }

        public override int CountPeople(
            Expression<Func<Person, bool>> predicate)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.Count(predicate);
            }
        }

        public override Person GetPerson(
            Guid id)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.Get(id).Clone();
            }
        }

        public override Person GetPersonWithAssociations(
            Guid id)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.GetPersonIncludingAssociations(id);
            }
        }

        public override IList<Person> GetAllPeople()
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.GetAll().ToList();
            }
        }

        public override IList<PersonAssociation> GetAllPersonAssociations()
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.PersonAssociations.GetAll()
                    .Select(pa => pa.Clone())
                    .ToList();
            }
        }

        public override IList<Person> FindPeople(
            Expression<Func<Person, bool>> predicate)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.Find(predicate).ToList();
            }
        }

        public override IList<Person> FindPeople(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.Find(predicates).ToList();
            }
        }

        public override IList<PersonAssociation> FindPersonAssociations(
            Expression<Func<PersonAssociation, bool>> predicate)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.PersonAssociations.Find(predicate).ToList();
            }
        }

        public override IList<PersonAssociation> FindPersonAssociations(
            IList<Expression<Func<PersonAssociation, bool>>> predicates)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.PersonAssociations.Find(predicates).ToList();
            }
        }

        public override void UpdatePerson(
            Person person)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePeople(
            IList<Person> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.UpdateRange(people);
                unitOfWork.Complete();
            }

            OnPeopleUpdated(people);
        }

        public override void UpdatePersonAssociation(
            PersonAssociation personAssociation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.PersonAssociations.Update(personAssociation);
                unitOfWork.Complete();
            }
        }

        public override void DeletePerson(
            Person person)
        {
            throw new NotImplementedException();
        }

        public override void DeletePeople(
            IList<Person> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = people.Select(p => p.Id).ToList();

                var peopleForDeletion = unitOfWork.People
                    .GetPeopleIncludingAssociations(p => ids.Contains(p.Id))
                    .ToList();

                var personAssociationsForDeletion = peopleForDeletion
                    .SelectMany(p => p.ObjectPeople)
                    .Concat(peopleForDeletion.SelectMany(p => p.SubjectPeople))
                    .ToList();

                unitOfWork.PersonAssociations.RemoveRange(personAssociationsForDeletion);
                unitOfWork.People.RemoveRange(peopleForDeletion);
                unitOfWork.Complete();
            }

            OnPeopleDeleted(people);
        }

        public override void DeletePersonAssociations(
            IList<PersonAssociation> personAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = personAssociations.Select(p => p.Id).ToList();
                var forDeletion = unitOfWork.PersonAssociations.Find(pa => ids.Contains(pa.Id));

                unitOfWork.PersonAssociations.RemoveRange(forDeletion);
                unitOfWork.Complete();
            }
        }

        protected override void LoadPeople(
            IList<Person> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.AddRange(people);
                unitOfWork.Complete();
            }
        }

        protected override void LoadPersonAssociations(
            IList<PersonAssociation> personAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.PersonAssociations.AddRange(personAssociations);
                unitOfWork.Complete();
            }
        }
    }
}
