using Craft.Utils;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using ApplicationState = Craft.DataStructures.Graph.State;

namespace Game.Rocket.ViewModel
{
    public class UnlockedLevelsViewModel : ViewModelBase
    {
        public ObservableCollection<Level> UnlockedLevels { get; }

        public UnlockedLevelsViewModel()
        {
            UnlockedLevels = new ObservableCollection<Level>();
        }
    }
}
