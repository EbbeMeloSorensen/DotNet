namespace PR.Persistence
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GenerateUnitOfWork();

        void Reseed();
    }
}
