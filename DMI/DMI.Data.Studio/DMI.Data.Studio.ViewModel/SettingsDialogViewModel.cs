using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;
using Craft.ViewModel.Utils;
using Craft.ViewModels.TrafficLight;

namespace DMI.Data.Studio.ViewModel
{
    public class SettingsDialogViewModel : DialogViewModelBase
    {
        private Brush _trafficLightBrushNeutral = new SolidColorBrush(Colors.DarkGray);
        private Brush _trafficLightBrushRed = new SolidColorBrush(Colors.Red);
        private Brush _trafficLightBrushGreen = new SolidColorBrush(Colors.Green);

        private readonly SMS.Application.IUIDataProvider _smsDataProvider;
        private readonly StatDB.Application.IUIDataProvider _statDBDataProvider;

        private string _smsDatabaseHostPersisted;
        private string _smsDatabaseUserIDPersisted;
        private string _smsDatabasePasswordPersisted;
        private string _smsDatabaseHost;
        private string _smsDatabaseUser;
        private string _smsDatabasePassword;
        private string _statdbDatabaseHostPersisted;
        private string _statdbDatabaseUserIDPersisted;
        private string _statdbDatabasePasswordPersisted;
        private string _statdbDatabaseHost;
        private string _statdbDatabaseUser;
        private string _statdbDatabasePassword;
        private bool _isBusyTestingSMSDatabaseConnection;
        private bool _isBusyTestingStatDBDatabaseConnection;
        private AsyncCommand _checkSMSDatabaseConnectionCommand;
        private AsyncCommand _checkStatDBDatabaseConnectionCommand;
        private RelayCommand _saveSettingsCommand;

        public string SmsDatabaseHost
        {
            get { return _smsDatabaseHost; }
            set
            {
                _smsDatabaseHost = value;
                RaisePropertyChanged();
                CheckSMSDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string SmsDatabaseUser
        {
            get { return _smsDatabaseUser; }
            set
            {
                _smsDatabaseUser = value;
                RaisePropertyChanged();
                CheckSMSDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string SmsDatabasePassword
        {
            get { return _smsDatabasePassword; }
            set
            {
                _smsDatabasePassword = value;
                RaisePropertyChanged();
                CheckSMSDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatDBDatabaseHost
        {
            get { return _statdbDatabaseHost; }
            set
            {
                _statdbDatabaseHost = value;
                RaisePropertyChanged();
                CheckStatDBDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatDBDatabaseUser
        {
            get { return _statdbDatabaseUser; }
            set
            {
                _statdbDatabaseUser = value;
                RaisePropertyChanged();
                CheckStatDBDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatDBDatabasePassword
        {
            get { return _statdbDatabasePassword; }
            set
            {
                _statdbDatabasePassword = value;
                RaisePropertyChanged();
                CheckStatDBDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsBusyTestingSMSDatabaseConnection
        {
            get { return _isBusyTestingSMSDatabaseConnection; }
            set
            {
                _isBusyTestingSMSDatabaseConnection = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusyTestingStatDBDatabaseConnection
        {
            get { return _isBusyTestingStatDBDatabaseConnection; }
            set
            {
                _isBusyTestingStatDBDatabaseConnection = value;
                RaisePropertyChanged();
            }
        }

        public AsyncCommand CheckSMSDatabaseConnectionCommand
        {
            get { return _checkSMSDatabaseConnectionCommand ?? (_checkSMSDatabaseConnectionCommand = new AsyncCommand(CheckSMSDatabaseConnection, CanCheckSMSDatabaseConnection)); }
        }

        public AsyncCommand CheckStatDBDatabaseConnectionCommand
        {
            get { return _checkStatDBDatabaseConnectionCommand ?? (_checkStatDBDatabaseConnectionCommand = new AsyncCommand(CheckStatDBDatabaseConnection, CanCheckStatDBDatabaseConnection)); }
        }

        public RelayCommand SaveSettingsCommand
        {
            get { return _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(SaveSettings, CanSaveSettings)); }
        }

        public TrafficLightViewModel TrafficLightViewModel_SMSDatabase { get; private set; }

        public TrafficLightViewModel TrafficLightViewModel_StatDBDatabase { get; private set; }

        public SettingsDialogViewModel(
            SMS.Application.IUIDataProvider smsDataProvider,
            StatDB.Application.IUIDataProvider statDBDataProvider)
        {
            _smsDataProvider = smsDataProvider;
            _statDBDataProvider = statDBDataProvider;

            TrafficLightViewModel_SMSDatabase = new TrafficLightViewModel(25);
            TrafficLightViewModel_StatDBDatabase = new TrafficLightViewModel(25);

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            var smsDatabaseHost = settings["SMS_PostgreSQL_Host"]?.Value;
            var smsDatabase = settings["SMS_PostgreSQL_Database"]?.Value;
            var smsSchema = settings["SMS_PostgreSQL_Schema"]?.Value;
            var smsUserID = settings["SMS_PostgreSQL_UserID"]?.Value;
            var smsPassword = settings["SMS_PostgreSQL_Password"]?.Value;

            SmsDatabaseHost = smsDatabaseHost;
            SmsDatabaseUser = smsUserID;
            SmsDatabasePassword = smsPassword;

            var statDBDatabaseHost = settings["StatDB_PostgreSQL_Host"]?.Value;
            var statDBDatabase = settings["StatDB_PostgreSQL_Database"]?.Value;
            var statDBSchema = settings["StatDB_PostgreSQL_Schema"]?.Value;
            var statDBUserID = settings["StatDB_PostgreSQL_UserID"]?.Value;
            var statDBPassword = settings["StatDB_PostgreSQL_Password"]?.Value;

            StatDBDatabaseHost = statDBDatabaseHost;
            StatDBDatabaseUser = statDBUserID;
            StatDBDatabasePassword = statDBPassword;

            InitializePersistedVariables();
        }

        private async Task CheckSMSDatabaseConnection()
        {
            try
            {
                IsBusyTestingSMSDatabaseConnection = true;
                CheckSMSDatabaseConnectionCommand.RaiseCanExecuteChanged();

                TrafficLightViewModel_SMSDatabase.Brush = _trafficLightBrushNeutral;

                SaveSettings();

                TrafficLightViewModel_SMSDatabase.Brush = await _smsDataProvider.CheckConnection()
                    ? _trafficLightBrushGreen
                    : _trafficLightBrushRed;
            }
            finally
            {
                IsBusyTestingSMSDatabaseConnection = false;
                CheckSMSDatabaseConnectionCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanCheckSMSDatabaseConnection()
        {
            return
                !IsBusyTestingSMSDatabaseConnection &&
                !string.IsNullOrEmpty(SmsDatabaseHost) &&
                !string.IsNullOrEmpty(SmsDatabaseUser) &&
                !string.IsNullOrEmpty(SmsDatabasePassword);
        }

        private async Task CheckStatDBDatabaseConnection()
        {
            try
            {
                IsBusyTestingStatDBDatabaseConnection = true;
                CheckStatDBDatabaseConnectionCommand.RaiseCanExecuteChanged();

                TrafficLightViewModel_StatDBDatabase.Brush = _trafficLightBrushNeutral;

                SaveSettings();

                TrafficLightViewModel_StatDBDatabase.Brush = await _statDBDataProvider.CheckConnection()
                    ? _trafficLightBrushGreen
                    : _trafficLightBrushRed;
            }
            finally
            {
                IsBusyTestingStatDBDatabaseConnection = false;
                CheckStatDBDatabaseConnectionCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanCheckStatDBDatabaseConnection()
        {
            return
                !IsBusyTestingStatDBDatabaseConnection &&
                !string.IsNullOrEmpty(StatDBDatabaseHost) &&
                !string.IsNullOrEmpty(StatDBDatabaseUser) &&
                !string.IsNullOrEmpty(StatDBDatabasePassword);
        }

        private void SaveSettings()
        {
            AddOrUpdateAppSettings("SMS_PostgreSQL_Host", SmsDatabaseHost);
            AddOrUpdateAppSettings("SMS_PostgreSQL_UserID", SmsDatabaseUser);
            AddOrUpdateAppSettings("SMS_PostgreSQL_Password", SmsDatabasePassword);
            AddOrUpdateAppSettings("StatDB_PostgreSQL_Host", StatDBDatabaseHost);
            AddOrUpdateAppSettings("StatDB_PostgreSQL_UserID", StatDBDatabaseUser);
            AddOrUpdateAppSettings("StatDB_PostgreSQL_Password", StatDBDatabasePassword);

            InitializePersistedVariables();
            SaveSettingsCommand.RaiseCanExecuteChanged();
        }

        private bool CanSaveSettings()
        {
            return
                _smsDatabaseHostPersisted != SmsDatabaseHost ||
                _smsDatabaseUserIDPersisted != SmsDatabaseUser ||
                _smsDatabasePasswordPersisted != SmsDatabasePassword ||
                _statdbDatabaseHostPersisted != StatDBDatabaseHost ||
                _statdbDatabaseUserIDPersisted != StatDBDatabaseUser ||
                _statdbDatabasePasswordPersisted != StatDBDatabasePassword;
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

        private void InitializePersistedVariables()
        {
            _smsDatabaseHostPersisted = SmsDatabaseHost;
            _smsDatabaseUserIDPersisted = SmsDatabaseUser;
            _smsDatabasePasswordPersisted = SmsDatabasePassword;

            _statdbDatabaseHostPersisted = StatDBDatabaseHost;
            _statdbDatabaseUserIDPersisted = StatDBDatabaseUser;
            _statdbDatabasePasswordPersisted = StatDBDatabasePassword;
        }
    }
}
