namespace DMI.ObsDB.Persistence
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GenerateUnitOfWork();
    }
}
