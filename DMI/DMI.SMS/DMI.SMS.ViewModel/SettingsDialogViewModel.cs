using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;
using Craft.ViewModel.Utils;
using Craft.ViewModels.TrafficLight;
using DMI.SMS.Application;
using DMI.SMS.Persistence;

namespace DMI.SMS.ViewModel
{
    public class SettingsDialogViewModel : DialogViewModelBase
    {
        private Brush _trafficLightBrushNeutral = new SolidColorBrush(Colors.DarkGray);
        private Brush _trafficLightBrushRed = new SolidColorBrush(Colors.Red);
        private Brush _trafficLightBrushGreen = new SolidColorBrush(Colors.Green);

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private string _databaseHostPersisted;
        private string _databasePortPersisted;
        private string _databaseUserIDPersisted;
        private string _databasePasswordPersisted;
        private string _databaseHost;
        private string _databasePort;
        private string _databaseUser;
        private string _databasePassword;
        private bool _isBusyTestingDatabaseConnection;
        private AsyncCommand _checkDatabaseConnectionCommand;
        private RelayCommand _saveSettingsCommand;

        public string DatabaseHost
        {
            get { return _databaseHost; }
            set
            {
                _databaseHost = value;
                RaisePropertyChanged();
                CheckDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string DatabasePort
        {
            get { return _databasePort; }
            set
            {
                _databasePort = value;
                RaisePropertyChanged();
                CheckDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string DatabaseUser
        {
            get { return _databaseUser; }
            set
            {
                _databaseUser = value;
                RaisePropertyChanged();
                CheckDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public string DatabasePassword
        {
            get { return _databasePassword; }
            set
            {
                _databasePassword = value;
                RaisePropertyChanged();
                CheckDatabaseConnectionCommand.RaiseCanExecuteChanged();
                SaveSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsBusyTestingDatabaseConnection
        {
            get { return _isBusyTestingDatabaseConnection; }
            set
            {
                _isBusyTestingDatabaseConnection = value;
                RaisePropertyChanged();
            }
        }

        public AsyncCommand CheckDatabaseConnectionCommand
        {
            get { return _checkDatabaseConnectionCommand ?? (_checkDatabaseConnectionCommand = new AsyncCommand(CheckDatabaseConnection, CanCheckDatabaseConnection)); }
        }

        public RelayCommand SaveSettingsCommand
        {
            get { return _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(SaveSettings, CanSaveSettings)); }
        }

        public TrafficLightViewModel TrafficLightViewModel_Database { get; private set; }

        public SettingsDialogViewModel(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;

            TrafficLightViewModel_Database = new TrafficLightViewModel(25);

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            var databaseHost = settings["SMS_PostgreSQL_Host"]?.Value;
            var port = settings["SMS_PostgreSQL_Port"]?.Value;
            var database = settings["SMS_PostgreSQL_Database"]?.Value;
            var schema = settings["SMS_PostgreSQL_Schema"]?.Value;
            var userID = settings["SMS_PostgreSQL_UserID"]?.Value;
            var password = settings["SMS_PostgreSQL_Password"]?.Value;

            DatabaseHost = databaseHost;
            DatabasePort = port;
            DatabaseUser = userID;
            DatabasePassword = password;

            InitializePersistedVariables();
        }

        private async Task CheckDatabaseConnection()
        {
            try
            {
                IsBusyTestingDatabaseConnection = true;
                CheckDatabaseConnectionCommand.RaiseCanExecuteChanged();

                TrafficLightViewModel_Database.Brush = _trafficLightBrushNeutral;

                SaveSettings();

                TrafficLightViewModel_Database.Brush = await _unitOfWorkFactory.CheckRepositoryConnection()
                    ? _trafficLightBrushGreen
                    : _trafficLightBrushRed;
            }
            finally
            {
                IsBusyTestingDatabaseConnection = false;
                CheckDatabaseConnectionCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanCheckDatabaseConnection()
        {
            return
                !IsBusyTestingDatabaseConnection &&
                !string.IsNullOrEmpty(DatabaseHost) &&
                !string.IsNullOrEmpty(DatabaseUser) &&
                !string.IsNullOrEmpty(DatabasePassword);
        }

        private void SaveSettings()
        {
            AddOrUpdateAppSettings("SMS_PostgreSQL_Host", DatabaseHost);
            AddOrUpdateAppSettings("SMS_PostgreSQL_Port", DatabasePort);
            AddOrUpdateAppSettings("SMS_PostgreSQL_UserID", DatabaseUser);
            AddOrUpdateAppSettings("SMS_PostgreSQL_Password", DatabasePassword);

            InitializePersistedVariables();
            SaveSettingsCommand.RaiseCanExecuteChanged();
        }

        private bool CanSaveSettings()
        {
            return
                _databaseHostPersisted != DatabaseHost ||
                _databasePortPersisted != DatabasePort ||
                _databaseUserIDPersisted != DatabaseUser ||
                _databasePasswordPersisted != DatabasePassword;
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
            _databaseHostPersisted = DatabaseHost;
            _databasePortPersisted = DatabasePort;
            _databaseUserIDPersisted = DatabaseUser;
            _databasePasswordPersisted = DatabasePassword;
        }
    }
}
