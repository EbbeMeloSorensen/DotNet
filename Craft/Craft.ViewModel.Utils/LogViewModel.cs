using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Craft.ViewModel.Utils
{
    public class LogViewModel : ViewModelBase
    {
        private string _log;
        private bool _logUpdated;
        private RelayCommand _clearLogsCommand;

        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged();
            }
        }

        public bool LogUpdated
        {
            get { return _logUpdated; }
            set
            {
                _logUpdated = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ClearLogsCommand
        {
            get { return _clearLogsCommand ?? (_clearLogsCommand = new RelayCommand(ClearLogs)); }
        }

        private void ClearLogs()
        {
            Log = "";
        }
    }
}
