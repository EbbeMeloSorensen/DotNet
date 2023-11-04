using System.Collections.ObjectModel;
using Craft.Utils;
using GalaSoft.MvvmLight;
using Simulator.Application;

namespace Simulator.ViewModel
{
    public class ApplicationStateListViewModel : ViewModelBase
    {
        private Craft.DataStructures.State _currentApplicationState;

        public ObservableObject<Craft.DataStructures.State> SelectedApplicationState { get; }

        public ObservableCollection<Craft.DataStructures.State> ApplicationStates { get; }

        public Craft.DataStructures.State CurrentApplicationState
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
            ApplicationStates = new ObservableCollection<Craft.DataStructures.State>();
            SelectedApplicationState = new ObservableObject<Craft.DataStructures.State>();
        }

        public ApplicationStateListViewModel(
            Simulator.Application.Application application) : this() // Call the default constructor
        {
            // (Method group)
            application.ApplicationStates.ForEach(AddApplicationState);
        }

        public void AddApplicationState(
            Craft.DataStructures.State applicationState)
        {
            ApplicationStates.Add(applicationState);
        }
    }
}