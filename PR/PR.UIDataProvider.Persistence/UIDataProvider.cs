using System.Collections.Generic;
using System.Linq;
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
    }
}
