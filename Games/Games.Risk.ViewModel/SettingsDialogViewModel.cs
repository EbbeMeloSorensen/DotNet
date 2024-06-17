using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;

namespace Games.Risk.ViewModel
{
    public class SettingsDialogViewModel : DialogViewModelBase
    {
        private int _playerCount;
        private readonly int _playerCountPersisted;
        private int _humanPlayerCount;
        private readonly int _humanPlayerCountPersisted;
        private int _delay;
        private readonly int _delayPersisted;
        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public ObservableCollection<int> PlayerCountOptions { get; }
        public ObservableCollection<int> HumanPlayerCountOptions { get; }
        public ObservableCollection<int> DelayOptions { get; }

        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value;
                OKCommand.RaiseCanExecuteChanged();
            }
        }

        public int HumanPlayerCount
        {
            get => _humanPlayerCount;
            set
            {
                _humanPlayerCount = value;
                OKCommand.RaiseCanExecuteChanged();
            }
        }

        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OKCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<object> OKCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand<object>(OK, CanOK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel)); }
        }

        public SettingsDialogViewModel(
            int playerCount,
            int humanPlayerCount,
            int delay)
        {
            PlayerCountOptions = new ObservableCollection<int>(Enumerable.Range(2, 5));
            HumanPlayerCountOptions = new ObservableCollection<int>(Enumerable.Range(2, 5));
            DelayOptions = new ObservableCollection<int>(new[] { 0, 100, 200, 300, 500, 800, 1300 });

            PlayerCount = playerCount;
            HumanPlayerCount = humanPlayerCount;
            Delay = delay;

            _playerCountPersisted = PlayerCount;
            _humanPlayerCountPersisted = HumanPlayerCount;
            _delayPersisted = Delay;
        }

        private void OK(
            object parameter)
        {
            AddOrUpdateAppSettings("PlayerCount", $"{PlayerCount}");
            AddOrUpdateAppSettings("HumanPlayerCount", $"{HumanPlayerCount}");
            AddOrUpdateAppSettings("Delay", $"{Delay}");
            CloseDialogWithResult(parameter as Window, DialogResult.OK);
        }

        private bool CanOK(
            object parameter)
        {
            return
                PlayerCount != _playerCountPersisted ||
                HumanPlayerCount != _humanPlayerCountPersisted ||
                Delay != _delayPersisted;
        }

        private void Cancel(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }

        private void AddOrUpdateAppSettings(
            string key,
            string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
                throw;
            }
        }
    }
}