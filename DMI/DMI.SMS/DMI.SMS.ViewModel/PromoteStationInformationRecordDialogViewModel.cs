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
    public class PromoteStationInformationRecordDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private string _dateFrom;
        private string _dateTo;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public string DateFrom
        {
            get
            {
                return _dateFrom;
            }
            set
            {
                _dateFrom = value;
                RaisePropertyChanged();
            }
        }

        public string DateTo
        {
            get
            {
                return _dateTo;
            }
            set
            {
                _dateTo = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> OKCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand<object>(OK, CanOK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel, CanCancel)); }
        }

        public PromoteStationInformationRecordDialogViewModel(
            DateTime? defaultDateFrom,
            DateTime? defaultDateTo)
        {
            if (defaultDateFrom.HasValue)
            {
                DateFrom = defaultDateFrom.Value.AsDateTimeString(true, true);
            }

            if (defaultDateTo.HasValue)
            {
                DateTo = defaultDateTo.Value.AsDateTimeString(true, true);
            }
        }

        private void OK(object parameter)
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            DateFrom = DateFrom.NullifyIfEmpty();
            DateTo = DateTo.NullifyIfEmpty();

            CloseDialogWithResult(parameter as Window, DialogResult.OK);
        }

        private bool CanOK(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }

        private bool CanCancel(object parameter)
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
                        case "DateFrom":
                            {
                                if (string.IsNullOrEmpty(DateFrom))
                                {
                                    errorMessage = "Required";
                                }
                                else if (!DateFrom.IsProperlyFormattedAsADateTime())
                                {
                                    errorMessage = "Format must be yyyy-mm-dd hh.mm.ss.fff";
                                }
                                else if (!DateFrom.TryParsingAsDateTime(out var dateTime))
                                {
                                    errorMessage = "Must be a valid date";
                                }

                                break;
                            }
                        case "DateTo":
                            {
                                if (string.IsNullOrEmpty(DateTo))
                                {
                                    errorMessage = "Required";
                                }
                                else if (!DateTo.IsProperlyFormattedAsADateTime())
                                {
                                    errorMessage = "Format must be yyyy-mm-dd hh.mm.ss.fff";
                                }
                                else if (!DateTo.TryParsingAsDateTime(out var dateTime))
                                {
                                    errorMessage = "Must be a valid date";
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
                        new ValidationError {PropertyName = "DateFrom"},
                        new ValidationError {PropertyName = "DateTo"},
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
            RaisePropertyChanged("DateFrom");
            RaisePropertyChanged("DateTo");
        }

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }
    }
}
