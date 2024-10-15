namespace PR.Persistence
{
    public interface IUnitOfWorkFactory
    {
        void Initialize(
            bool versioned);

        IUnitOfWork GenerateUnitOfWork();

        void Reseed();
    }
}
