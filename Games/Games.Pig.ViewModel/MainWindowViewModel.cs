using System;
using GalaSoft.MvvmLight.Command;
using Games.Pig.Application;

namespace Games.Pig.ViewModel
{
    public class MainWindowViewModel
    {
        private Engine _engine;

        private RelayCommand _startGameCommand;
        private RelayCommand _rollDieCommand;
        private RelayCommand _takePotCommand;

        public RelayCommand StartGameCommand => _startGameCommand ??= new RelayCommand(StartGame, CanStartGame);
        public RelayCommand RollDieCommand => _rollDieCommand ??= new RelayCommand(RollDie, CanRollDie);
        public RelayCommand TakePotCommand => _rollDieCommand ??= new RelayCommand(TakePot, CanTakePot);

        public MainWindowViewModel()
        {
            var players = new[] { true, false };
            _engine = new Engine(players);
        }

        private void StartGame()
        {
            throw new NotImplementedException();
        }

        private bool CanStartGame()
        {
            return true;
        }

        private void RollDie()
        {
            throw new NotImplementedException();
        }

        private bool CanRollDie()
        {
            return true;
        }

        private void TakePot()
        {
            throw new NotImplementedException();
        }

        private bool CanTakePot()
        {
            return true;
        }
    }
}
