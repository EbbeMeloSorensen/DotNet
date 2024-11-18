using System;
using Craft.Logging;

namespace PR.Persistence.Versioned
{
    public class UnitOfWorkFactoryFacade : IUnitOfWorkFactoryVersioned, IUnitOfWorkFactoryHistorical
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DateTime? DatabaseTime { get; set; }
        public DateTime? HistoricalTime { get; set; }
        public bool IncludeCurrentObjects { get; set; }
        public bool IncludeHistoricalObjects { get; set; }

        public ILogger Logger { get; set; }

        public UnitOfWorkFactoryFacade(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            Logger = _unitOfWorkFactory.Logger;
        }

        public void Initialize(
            bool versioned)
        {
            _unitOfWorkFactory.Initialize(versioned);
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWorkFacade(
                Logger,
                _unitOfWorkFactory.GenerateUnitOfWork(),
                HistoricalTime,
                DatabaseTime,
                IncludeCurrentObjects,
                IncludeHistoricalObjects);
        }

        public void Reseed()
        {
            _unitOfWorkFactory.Reseed();
        }
    }
}
