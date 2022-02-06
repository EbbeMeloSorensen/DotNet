using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Craft.UI.Utils;

namespace DMI.SMS.ViewModel
{
    public class ExtractFrieDataStationListDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private string _date;

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

        public RelayCommand<object> OKCommand { get; }
        public RelayCommand<object> CancelCommand { get; }

        public ExtractFrieDataStationListDialogViewModel(
            string dialogTitle)
        {
            DialogTitle = dialogTitle;
            Date = "";

            OKCommand = new RelayCommand<object>(OK, CanOK);
            CancelCommand = new RelayCommand<object>(Cancel, CanCancel);
        }

        private void OK(
            object parameter)
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            Date = Date.NullifyIfEmpty();

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
            return true;
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
