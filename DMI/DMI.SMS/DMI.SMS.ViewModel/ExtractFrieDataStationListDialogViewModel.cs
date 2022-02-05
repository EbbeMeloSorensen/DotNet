using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;

namespace DMI.SMS.ViewModel
{
    public class ExtractFrieDataStationListDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private Application.Application _application;
        private string _date;
        private bool _isBusy;
        private double _progress;
        private string _currentActivity;
        private bool _abort;

        public string DialogTitle { get; }

        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => Set(ref _isBusy, value);
        }

        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentActivity
        {
            get
            {
                return _currentActivity;
            }
            set
            {
                _currentActivity = value;
                RaisePropertyChanged();
            }
        }

        public AsyncCommand ExtractCommand { get; }
        public RelayCommand AbortCommand { get; }

        public RelayCommand<object> OKCommand { get; }
        public RelayCommand<object> CancelCommand { get; }

        public ExtractFrieDataStationListDialogViewModel(
            Application.Application application,
            string dialogTitle)
        {
            _application = application; 
            DialogTitle = dialogTitle;
            Date = "";

            ExtractCommand = new AsyncCommand(Extract, CanExtract);
            AbortCommand = new RelayCommand(Abort, CanAbort);
            OKCommand = new RelayCommand<object>(OK, CanOK);
            CancelCommand = new RelayCommand<object>(Cancel, CanCancel);
        }

        private async Task Extract()
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            Date = Date.NullifyIfEmpty();

            _abort = false;
            Progress = 0;
            IsBusy = true;
            ExtractCommand.RaiseCanExecuteChanged();
            AbortCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();

            DateTime? date = null;

            if (!string.IsNullOrEmpty(Date))
            {
                Date.TryParsingAsDateTime(out var temp);
                date = temp;
            }

            await _application.ExtractFrieDataMeteorologicalStationList(
                date,
                (progress, currentActivity) =>
                {
                    Progress = progress;
                    CurrentActivity = currentActivity;
                    return _abort;
                });

            IsBusy = false;
            ExtractCommand.RaiseCanExecuteChanged();
            AbortCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        private bool CanExtract()
        {
            return !IsBusy;
        }

        private void Abort()
        {
            _abort = true;
        }

        private bool CanAbort()
        {
            return IsBusy;
        }

        private void OK(
            object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.OK);
        }

        private bool CanOK(
            object parameter)
        {
            return true;
        }

        private void Cancel(
            object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }

        private bool CanCancel(
            object parameter)
        {
            return !IsBusy;
        }

        public string this[string columnName]
        {
            get
            {
                var errorMessage = string.Empty;

                if (_state == StateOfView.Updated)
                {
                    switch (columnName)
                    {
                        case "Date":
                            {
                                if (!string.IsNullOrEmpty(Date))
                                {
                                    if (!Date.IsProperlyFormattedAsADate())
                                    {
                                        errorMessage = "Format must be yyyy-mm-dd";
                                    }
                                    else if (!Date.TryParsingAsDateTime(out var dateTime))
                                    {
                                        errorMessage = "Must be a valid date";
                                    }
                                }

                                break;
                            }
                        default:
                            throw new ArgumentException("Invalid column name encountered while validating input for station information creation");
                    }
                }

                ValidationMessages
                    .First(e => e.PropertyName == columnName).ErrorMessage = errorMessage;

                return errorMessage;
            }
        }

        public ObservableCollection<ValidationError> ValidationMessages
        {
            get
            {
                if (_validationMessages == null)
                {
                    _validationMessages = new ObservableCollection<ValidationError>
                    {
                        new ValidationError {PropertyName = "Date"},
                    };
                }

                return _validationMessages;
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged();
            }
        }

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged("Date");
        }

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }
    }
}
