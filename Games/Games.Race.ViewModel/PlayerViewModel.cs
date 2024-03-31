using GalaSoft.MvvmLight;

namespace Games.Race.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private int _score;
        private bool _hasInitiative;

        public string Name { get; set; }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                RaisePropertyChanged();
            }
        }

        public bool HasInitiative
        {
            get => _hasInitiative;
            set
            {
                _hasInitiative = value;
                RaisePropertyChanged();
            }
        }
    }
}
