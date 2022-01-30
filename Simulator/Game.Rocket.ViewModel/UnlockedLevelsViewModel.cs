using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Simulator.Application;
using Simulator.ViewModel;

namespace Game.Rocket.ViewModel
{
    public class UnlockedLevelsViewModel : ViewModelBase
    {
        public ApplicationStateListViewModel ApplicationStateListViewModel { get; }

        public UnlockedLevelsViewModel()
        {
            ApplicationStateListViewModel = new ApplicationStateListViewModel();
        }

        public void UnlockLevel(
            Level level)
        {
            if (ApplicationStateListViewModel.ApplicationStates.Contains(level)) return;

            ApplicationStateListViewModel.AddApplicationState(level);
        }
    }
}
