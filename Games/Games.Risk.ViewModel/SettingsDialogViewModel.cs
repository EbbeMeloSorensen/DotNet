using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

namespace Games.Risk.ViewModel
{
    public class SettingsDialogViewModel : DialogViewModelBase
    {
        private int _playerCount;
        private int _playerCountPersisted;
        private RelayCommand _saveSettingsCommand;

        public ObservableCollection<int> PlayerCountOptions { get; }

        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value;
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SaveSettingsCommand
        {
            get { return _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(SaveSettings, CanSaveSettings)); }
        }

        public SettingsDialogViewModel()
        {
            PlayerCountOptions = new ObservableCollection<int>(Enumerable.Range(2, 5));

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var playerCount = settings["PlayerCount"]?.Value;

            PlayerCount = int.Parse(playerCount);

            InitializePersistedVariables();
        }

        private void SaveSettings()
        {
            AddOrUpdateAppSettings("PlayerCount", $"{PlayerCount}");

            InitializePersistedVariables();
            SaveSettingsCommand.RaiseCanExecuteChanged();
        }

        private bool CanSaveSettings()
        {
            return PlayerCount != _playerCountPersisted;
        }

        private void InitializePersistedVariables()
        {
            _playerCountPersisted = PlayerCount;
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