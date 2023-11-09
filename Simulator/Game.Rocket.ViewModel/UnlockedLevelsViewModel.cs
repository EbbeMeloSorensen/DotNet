using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Game.Rocket.ViewModel
{
    public class UnlockedLevelsViewModel : ViewModelBase
    {
        private string _greeting = "Greetings from UnlockedLevelsViewModel";

        public string Greeting
        {
            get => _greeting;
            set
            {
                _greeting = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Level> UnlockedLevels { get; }

        public UnlockedLevelsViewModel()
        {
            UnlockedLevels = new ObservableCollection<Level>();
        }
    }
}
