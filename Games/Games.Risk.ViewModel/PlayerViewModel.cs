using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using Craft.Utils;
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

        public ObservableObject<List<Card>> SelectedCards { get; }
            
        public PlayerViewModel()
        {
            CardViewModels = new ObservableCollection<CardViewModel>();
            SelectedCards = new ObservableObject<List<Card>>();
        }

        public void AddCardViewModel(
            string territory,
            Card card,
            bool bottomSideUp)
        {
            var cardViewModel = new CardViewModel(card)
            {
                Territory = territory,
                Offset = CardViewModels.Count * 13,
                BottomSideUp = bottomSideUp
            };

            CardViewModels.Add(cardViewModel);

            cardViewModel.CardClicked += (s, e) =>
            {
                SelectedCards.Object = new List<Card>(
                    CardViewModels
                        .Where(_ => _.Selected)
                        .Select(_ => _.Card));
            };
        }
    }
}
