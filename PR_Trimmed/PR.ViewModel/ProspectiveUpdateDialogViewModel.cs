using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using System;
using System.Timers;
using System.Windows;

namespace PR.ViewModel
{
    public class ProspectiveUpdateDialogViewModel : DialogViewModelBase
    {
        private readonly Timer _timer;
        private ProspectiveUpdateType _prospectiveUpdateType;
        private bool _timeFieldEnabled;
        private string _timeOfChange;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public ProspectiveUpdateType ProspectiveUpdateType
        {
            get => _prospectiveUpdateType;
            set
            {
                _prospectiveUpdateType = value;
                RaisePropertyChanged();

                TimeFieldEnabled = _prospectiveUpdateType == ProspectiveUpdateType.Earlier;

                if (!TimeFieldEnabled)
                {
                    UpdateTime();
                }
            }
        }

        public bool TimeFieldEnabled
        {
            get => _timeFieldEnabled;
            set
            {
                _timeFieldEnabled = value;
                RaisePropertyChanged();
            }
        }

        public string TimeOfChange
        {
            get => _timeOfChange;
            set
            {
                _timeOfChange = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> OKCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand<object>(OK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel)); }
        }

        public ProspectiveUpdateDialogViewModel()
        {
            UpdateTime();

            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) => UpdateTime();
            _timer.Start();
        }

        private void OK(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.OK);
        }

        private void Cancel(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }

        private void UpdateTime()
        {
            if (ProspectiveUpdateType == ProspectiveUpdateType.Now)
            {
                TimeOfChange = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
