using Craft.Utils;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using ApplicationState = Craft.DataStructures.Graph.State;

namespace Game.Rocket.ViewModel
{
    public class ApplicationStateListViewModel : ViewModelBase
    {
        private ApplicationState _currentApplicationState;

        public ObservableObject<ApplicationState> SelectedApplicationState { get; }

        public ObservableCollection<ApplicationState> ApplicationStates { get; }

        public ApplicationState CurrentApplicationState
        {
            get { return _currentApplicationState; }
            set
            {
                _currentApplicationState = value;
                SelectedApplicationState.Object = _currentApplicationState;
                RaisePropertyChanged();
            }
        }

        public ApplicationStateListViewModel()
        {
            ApplicationStates = new ObservableCollection<ApplicationState>();
            SelectedApplicationState = new ObservableObject<ApplicationState>();
        }

        public ApplicationStateListViewModel(
            Simulator.Application.Application application) : this() // Call the default constructor
        {
            // (Method group)
            application.ApplicationStates.ForEach(AddApplicationState);
        }

        public void AddApplicationState(
            ApplicationState applicationState)
        {
            ApplicationStates.Add(applicationState);
        }
    }
}
