using System.Threading.Tasks;
using Craft.Logging;

namespace PR.Persistence
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GenerateUnitOfWork();
    }
}
