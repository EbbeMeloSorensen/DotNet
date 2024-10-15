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

        public void Initialize(
            bool versioned)
        {
            _unitOfWorkFactory.Initialize(versioned);
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWorkFacade(
                _unitOfWorkFactory.GenerateUnitOfWork(),
                DatabaseTime);
        }

        public void Reseed()
        {
            _unitOfWorkFactory.Reseed();
        }
    }
}
