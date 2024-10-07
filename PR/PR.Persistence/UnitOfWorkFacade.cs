using System;
using PR.Persistence.RepositoryFacades;

namespace PR.Persistence
{
    public class UnitOfWorkFacade : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonRepositoryFacade PersonRepository { get; }
        public PersonAssociationRepositoryFacade PersonAssociationRepository { get; }

        public UnitOfWorkFacade(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            PersonRepository = new PersonRepositoryFacade(_unitOfWork.People);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}