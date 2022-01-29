using System.Threading.Tasks;
using System.Windows.Input;

namespace Craft.ViewModel.Utils
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}