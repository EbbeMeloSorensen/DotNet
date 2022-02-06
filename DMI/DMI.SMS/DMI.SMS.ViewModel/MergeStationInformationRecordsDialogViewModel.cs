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
    public class MergeStationInformationRecordsDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private DateTime _startTime;
        private DateTime? _endTime;
        private string _startTimeAsString;
        private string _endTimeAsString;
        private string _transitionTime;

        private string _originalTransitionTime;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public string StartTime
        {
            get
            {
                return _startTimeAsString;
            }
            set
            {
                _startTimeAsString = value;
                RaisePropertyChanged();
            }
        }

        public string EndTime
        {
            get
            {
                return _endTimeAsString;
            }
            set
            {
                _endTimeAsString = value;
                RaisePropertyChanged();
            }
        }

        public string TransitionTime
        {
            get
            {
                return _transitionTime;
            }
            set
            {
                _transitionTime = value;
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

        public MergeStationInformationRecordsDialogViewModel(
            DateTime startTime,
            DateTime? endTime,
            DateTime defaultTransitionTime)
        {
            _startTime = startTime;
            StartTime = startTime.AsDateTimeString(true, true);

            _endTime = endTime;
            if (endTime.HasValue)
            {
                EndTime = endTime.Value.AsDateTimeString(true, true);
            }

            TransitionTime = defaultTransitionTime.AsDateTimeString(true, true);

            _originalTransitionTime = TransitionTime;
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

            TransitionTime = TransitionTime.NullifyIfEmpty();

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
                        case "TransitionTime":
                        {
                            if (string.IsNullOrEmpty(TransitionTime))
                            {
                                errorMessage = "Required";
                            }
                            else if (!TransitionTime.IsProperlyFormattedAsADateTime())
                            {
                                errorMessage = "Format must be yyyy-mm-dd hh.mm.ss.fff";
                            }
                            else if (!TransitionTime.TryParsingAsDateTime(out var dateTime))
                            {
                                errorMessage = "Must be a valid date";
                            }
                            else if (TransitionTime == _originalTransitionTime)
                            {
                                errorMessage = "Please correct the transition time";
                            }
                            else
                            {
                                TransitionTime.TryParsingAsDateTime(out var newTransitionTime);

                                if (_startTime > newTransitionTime)
                                {
                                    errorMessage = "New transition time must exceed start time";
                                }
                                else if (_endTime.HasValue && _endTime.Value < newTransitionTime)
                                {
                                    errorMessage = "New transition time may not exceed end time";
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
                        new ValidationError {PropertyName = "TransitionTime"},
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
            RaisePropertyChanged("TransitionTime");
        }

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }
    }
}