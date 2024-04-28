using GalaSoft.MvvmLight;
using Games.Risk.Application;

namespace Games.Risk.ViewModel
{
    public class CardViewModel : ViewModelBase
    {
        private string _territory;
        private bool _bottomSide;
        private double _offset;
        private string _cardTypeString;

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

        public string CardTypeString
        {
            get => _cardTypeString;
            set
            {
                _cardTypeString = value;
                RaisePropertyChanged();
            }
        }

        public CardViewModel(
            CardType cardType)
        {
            CardTypeString = cardType switch
            {
                CardType.Soldier => "s",
                CardType.Horse => "h",
                CardType.Cannon=> "c",
                CardType.Joker=> "j",
                _ => CardTypeString
            };
        }
    }
}
