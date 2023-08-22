namespace C2IEDM.Persistence;

public interface IUnitOfWorkFactory
{
    IUnitOfWork GenerateUnitOfWork();
}