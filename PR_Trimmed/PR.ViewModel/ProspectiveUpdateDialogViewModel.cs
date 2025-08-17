using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Domain.Entities.PR;

namespace PR.ViewModel
{
    public class ProspectiveUpdateDialogViewModel : DialogViewModelBase
    {
        private readonly Application.Application _application;
        private readonly Timer _timer;
        private ProspectiveUpdateType _prospectiveUpdateType;
        private bool _timeFieldEnabled;
        private string _timeOfChange;
        private List<Person> _people;
        private string _generalError;
        private bool _displayGeneralError;

        private AsyncCommand<object> _okCommand;
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

        public string GeneralError
        {
            get => _generalError;
            set
            {
                _generalError = value;
                DisplayGeneralError = !string.IsNullOrEmpty(_generalError);
                RaisePropertyChanged();
            }
        }

        public bool DisplayGeneralError
        {
            get => _displayGeneralError;
            set
            {
                _displayGeneralError = value;
                RaisePropertyChanged();
            }
        }

        public AsyncCommand<object> OKCommand
        {
            get { return _okCommand ?? (_okCommand = new AsyncCommand<object>(OK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel)); }
        }

        public ProspectiveUpdateDialogViewModel(
            Application.Application application,
            List<Person> people)
        {
            _application = application;
            _people = people;

            UpdateTime();

            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) => UpdateTime();
            _timer.Start();
        }

        private async Task OK(
            object parameter)
        {
            try
            {
                DateTime? timeOfChange = null;

                if (ProspectiveUpdateType == ProspectiveUpdateType.Earlier &&
                    TimeOfChange.TryParsingAsDateTime(out var temp))
                {
                    timeOfChange = temp;
                }

                await _application.UpdatePeople(_people, timeOfChange);

                CloseDialogWithResult(parameter as Window, DialogResult.OK);
            }
            catch (InvalidOperationException e)
            {
                GeneralError = e.Message;
            }
        }

        private void Cancel(
            object parameter)
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
