namespace PR.Persistence
{
    public class UnitOfWorkFactoryFacade
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public UnitOfWorkFactoryFacade(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public UnitOfWorkFacade GenerateUnitOfWork()
        {
            return new UnitOfWorkFacade(_unitOfWorkFactory.GenerateUnitOfWork());
        }
    }
}
