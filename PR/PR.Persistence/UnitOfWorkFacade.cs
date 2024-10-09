using System;
using PR.Persistence.RepositoryFacades;

namespace PR.Persistence
{
    public class UnitOfWorkFacade : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonRepositoryFacade People { get; }
        public PersonAssociationRepositoryFacade PersonAssociations { get; }

        public UnitOfWorkFacade(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            People = new PersonRepositoryFacade(_unitOfWork);
        }

        public void Complete()
        {
            _unitOfWork.Complete();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}