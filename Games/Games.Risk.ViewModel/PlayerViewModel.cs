using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Games.Risk.Application;

namespace Games.Risk.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
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

        public bool HasInitiative
        {
            get => _hasInitiative;
            set
            {
                _hasInitiative = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<CardViewModel> CardViewModels { get; }

        public PlayerViewModel()
        {
            CardViewModels = new ObservableCollection<CardViewModel>();
        }

        public void AddCardViewModel(
            string territory,
            CardType cardType)
        {
            var cardViewModel = new CardViewModel(cardType)
            {
                Territory = territory,
                Offset = CardViewModels.Count * 13
            };

            CardViewModels.Add(cardViewModel);

            cardViewModel.CardClicked += (s, e) =>
            {
                //throw new NotImplementedException();
            };
        }
    }
}
