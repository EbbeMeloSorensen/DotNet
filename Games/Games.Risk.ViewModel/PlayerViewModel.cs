using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Games.Risk.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private int _score;
        private bool _hasInitiative;
        private Brush _brush;

        public string Name { get; set; }

        public Brush Brush
        {
            get => _brush;
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

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
