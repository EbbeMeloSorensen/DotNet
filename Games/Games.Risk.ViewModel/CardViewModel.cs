using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Games.Risk.Application;

namespace Games.Risk.ViewModel
{
    public class CardViewModel : ViewModelBase
    {
        private string _territory;
        private bool _bottomSide;
        private double _offset;
        private string _cardTypeString;
        private bool _selected;
        private RelayCommand _clickedCommand;

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
