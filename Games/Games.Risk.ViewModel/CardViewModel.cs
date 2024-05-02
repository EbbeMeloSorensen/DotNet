using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Games.Risk.Application;

namespace Games.Risk.ViewModel
{
    public class CardViewModel : ViewModelBase
    {
        private string _territory;
        private bool _bottomSideUp;
        private double _offset;
        private string _cardTypeString;
        private bool _selected;
        private RelayCommand _clickedCommand;

        public Card Card { get; private set; }

        public string Territory
        {
            get => _territory;
            set
            {
                _territory = value;
                RaisePropertyChanged();
            }
        }

        public bool BottomSideUp
        {
            get => _bottomSideUp;
            set
            {
                _bottomSideUp = value;
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

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ClickedCommand
        {
            get
            {
                return _clickedCommand ?? (_clickedCommand = new RelayCommand(Clicked));
            }
        }

        public event EventHandler CardClicked;

        public CardViewModel(
            Card card)
        {
            Card = card;

            CardTypeString = card.Type switch
            {
                CardType.Soldier => "s",
                CardType.Horse => "h",
                CardType.Cannon=> "c",
                CardType.Joker=> "j",
                _ => CardTypeString
            };
        }

        private void Clicked()
        {
            OnCardClicked();
        }

        private void OnCardClicked()
        {
            Selected = !Selected;

            var handler = CardClicked;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
