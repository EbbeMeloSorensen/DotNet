using System.Threading.Tasks;

namespace DMI.Data.Studio.Application
{
    public interface IUIDataProvider
    {
        Task<bool> CheckConnection();
    }
}
