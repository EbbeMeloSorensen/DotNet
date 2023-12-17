using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.UI.Utils;
using Craft.ViewModels.Dialogs;

namespace C2IEDM.ViewModel
{
    public class DefineGeospatialLocationDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private double _latitude;
        private double _longitude;

        private DateTime _from;
        private DateTime? _to;

        // These are for limiting options for the DatePicker controls
        private DateTime _displayDateStart_DateFrom;
        private DateTime _displayDateEnd_DateFrom;
        private DateTime _displayDateStart_DateTo;
        private DateTime _displayDateEnd_DateTo;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                RaisePropertyChanged();
            }
        }

        public double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                RaisePropertyChanged();
            }
        }

        public DateTime From
        {
            get { return _from; }
            set
            {
                _from = value;
                RaisePropertyChanged();
            }
        }

        public DateTime? To
        {
            get { return _to; }
            set
            {
                _to = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DisplayDateStart_DateFrom
        {
            get => _displayDateStart_DateFrom;
            set
            {
                _displayDateStart_DateFrom = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DisplayDateEnd_DateFrom
        {
            get => _displayDateEnd_DateFrom;
            set
            {
                _displayDateEnd_DateFrom = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DisplayDateStart_DateTo
        {
            get => _displayDateStart_DateTo;
            set
            {
                _displayDateStart_DateTo = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DisplayDateEnd_DateTo
        {
            get => _displayDateEnd_DateTo;
            set
            {
                _displayDateEnd_DateTo = value;
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

        public string this[string columnName]
        {
            get
            {
                var errorMessage = string.Empty;

                if (_state == StateOfView.Updated)
                {
                    switch (columnName)
                    {
                        case "Latitude":
                        {
                            break;
                        }
                        case "Longitude":
                        {
                            break;
                        }
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
                        new ValidationError {PropertyName = "Latitude"},
                        new ValidationError {PropertyName = "Longitude"},
                        new ValidationError {PropertyName = "From"},
                        new ValidationError {PropertyName = "To"},
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

        public DefineGeospatialLocationDialogViewModel()
        {
            var currentDate = DateTime.Now.Date;
            DisplayDateEnd_DateFrom = currentDate;
            DisplayDateEnd_DateTo = currentDate;
            From = currentDate;
        }

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged("Latitude");
            RaisePropertyChanged("Longitude");
            RaisePropertyChanged("From");
            RaisePropertyChanged("To");
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

            CloseDialogWithResult(parameter as Window, DialogResult.OK);
        }

        private bool CanOK(object parameter)
        {
            return true;
            //return
            //    SubjectPerson != null &&
            //    ObjectPerson != null &&
            //    Description != null &&
            //    !string.IsNullOrEmpty(Description) &&
            //    (SubjectPerson != _originalSubjectPerson ||
            //     ObjectPerson != _originalObjectPerson ||
            //     Description != _originalDescription);
        }

        private void Cancel(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }

        private bool CanCancel(object parameter)
        {
            return true;
        }
    }
}
