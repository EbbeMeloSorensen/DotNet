namespace WIGOS.Persistence
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GenerateUnitOfWork();
    }
}