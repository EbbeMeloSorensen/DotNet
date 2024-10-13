using System;

namespace PR.Persistence.Versioned
{
    public class UnitOfWorkFactoryFacade : IUnitOfWorkFactory
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DateTime? DatabaseTime { get; set; }

        public UnitOfWorkFactoryFacade(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWorkFacade(
                _unitOfWorkFactory.GenerateUnitOfWork(),
                DatabaseTime);
        }

        public void Reseed()
        {
            throw new NotImplementedException();
        }
    }
}
