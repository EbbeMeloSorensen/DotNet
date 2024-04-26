using GalaSoft.MvvmLight;

namespace Games.Risk.ViewModel
{
    public enum CardType
    {
        Soldier,
        Horse,
        Cannon,
        Joker
    }

    public class CardViewModel : ViewModelBase
    {
        private string _territory;
        private bool _bottomSide;
        private double _offset;
        private CardType _cardType;

        public string Territory
        {
            get => _territory;
            set
            {
                _territory = value;
                RaisePropertyChanged();
            }
        }

        public bool BottomSide
        {
            get => _bottomSide;
            set
            {
                _bottomSide = value;
                RaisePropertyChanged();
            }
        }

        public double Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                RaisePropertyChanged();
            }
        }

        public CardType CardType
        {
            get => _cardType;
            set
            {
                _cardType = value;
                RaisePropertyChanged();
            }
        }
    }
}
