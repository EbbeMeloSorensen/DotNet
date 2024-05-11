using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using Craft.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Games.Risk.Application;

namespace Games.Risk.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private bool _hasInitiative;
        private Brush _brush;
        private bool _watchCardsButtonVisible;
        private string _watchCardsButtonText;
        private RelayCommand _toggleCardsVisibilityCommand;
        private int _armiesToDeploy;
        private string _armiesToDeployText;
        private bool _armiesToDeployTextVisible;

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

        public int ArmiesToDeploy
        {
            get => _armiesToDeploy;
            set
            {
                _armiesToDeploy = value;
                ArmiesToDeployText = $"Armies to deploy: {_armiesToDeploy}";
                ArmiesToDeployTextVisible = _armiesToDeploy > 0;
                RaisePropertyChanged();
            }
        }

        public string ArmiesToDeployText
        {
            get => _armiesToDeployText;
            set
            {
                _armiesToDeployText = value;
                RaisePropertyChanged();
            }
        }

        public bool ArmiesToDeployTextVisible
        {
            get => _armiesToDeployTextVisible;
            set
            {
                _armiesToDeployTextVisible = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<CardViewModel> CardViewModels { get; }

        public ObservableObject<List<Card>> SelectedCards { get; }

        public string WatchCardsButtonText
        {
            get => _watchCardsButtonText;
            set
            {
                _watchCardsButtonText = value;
                RaisePropertyChanged();
            }
        }

        public bool WatchCardsButtonVisible
        {
            get => _watchCardsButtonVisible;
            set
            {
                _watchCardsButtonVisible = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ToggleCardsVisibilityCommand
        {
            get
            {
                return _toggleCardsVisibilityCommand ?? (_toggleCardsVisibilityCommand = new RelayCommand(ToggleCardsVisibility));
            }
        }

        public PlayerViewModel()
        {
            CardViewModels = new ObservableCollection<CardViewModel>();
            SelectedCards = new ObservableObject<List<Card>>();
            WatchCardsButtonText = "Watch";
            ArmiesToDeploy = 0;
            ArmiesToDeployTextVisible = true;
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

        private void ToggleCardsVisibility()
        {
            CardViewModels.ToList().ForEach(_ =>
            {
                _.BottomSideUp = !_.BottomSideUp;
                _.Selected = false;
            });

            SelectedCards.Object = new List<Card>();

            WatchCardsButtonText = CardViewModels.First().BottomSideUp ? "Watch" : "Hide";
        }
    }
}
