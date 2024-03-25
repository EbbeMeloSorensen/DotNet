using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModel.Utils;
using Games.Pig.Application;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;

namespace Games.Pig.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Engine _engine;
        private int _pot;
        private int _computerScore;
        private int _playerScore;
        private bool _playerHasInitiative;

        private RelayCommand _startGameCommand;
        private AsyncCommand _rollDieCommand;
        private AsyncCommand _takePotCommand;

        public RelayCommand StartGameCommand => _startGameCommand ??= new RelayCommand(StartGame, CanStartGame);
        public AsyncCommand RollDieCommand => _rollDieCommand ??= new AsyncCommand(RollDie, CanRollDie);
        public AsyncCommand TakePotCommand => _takePotCommand ??= new AsyncCommand(TakePot, CanTakePot);

        public int Pot
        {
            get => _pot;
            private set
            {
                _pot = value;
                RaisePropertyChanged();
            }
        }

        public int ComputerScore
        {
            get => _computerScore;
            private set
            {
                _computerScore = value;
                RaisePropertyChanged();
            }
        }

        public int PlayerScore
        {
            get => _playerScore;
            private set
            {
                _playerScore = value;
                RaisePropertyChanged();
            }
        }

        public bool PlayerHasInitiative
        {
            get => _playerHasInitiative;
            private set
            {
                _playerHasInitiative = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            var players = new[] { true, false };
            _engine = new Engine(players);
            PlayerHasInitiative = !players[0];
        }

        private void StartGame()
        {
            throw new NotImplementedException();
        }

        private bool CanStartGame()
        {
            return true;
        }

        private async Task RollDie()
        {
            var gameEvent = await _engine.PlayerSelectsOption(new RollDie()) as PlayerRollsDie;

            Pot = _engine.Pot;
        }

        private bool CanRollDie()
        {
            return true;
        }

        private async Task TakePot()
        {
            var gameEvent = await _engine.PlayerSelectsOption(new TakePot()) as PlayerTakesPot;

            Pot = _engine.Pot;
        }

        private bool CanTakePot()
        {
            return true;
        }
    }
}
