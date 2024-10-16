namespace PR.Persistence.APIClient
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public void Initialize(bool versioned)
        {
            throw new System.NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork();
        }

        public void Reseed()
        {
            throw new System.NotImplementedException();
        }
    }
}
