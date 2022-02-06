using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Craft.UI.Utils;

namespace DMI.SMS.ViewModel
{
    public class CreateStationInformationDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        // Station ids must have 4 or 5 digits and not start with zero
        private static Regex regexStationId = new Regex(@"^[1-9](\d\d\d|\d\d\d\d)$");

        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private string _stationName;
        private string _stationid_dmi;
        private string _stationType;
        private string _country;
        private string _status;
        private string _dateFrom;
        private string _dateTo;
        private string _stationOwner;
        private string _wgs_lat;
        private string _wgs_long;
        private string _hha;
        private string _hhp;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public ObservableCollection<string> StationTypeOptions { get; }
        public ObservableCollection<string> StatusOptions { get; }
        public ObservableCollection<string> CountryOptions { get; }
        public ObservableCollection<string> StationOwnerOptions { get; }

        public string StationName
        {
            get { return _stationName; }
            set
            {
                _stationName = value;
                RaisePropertyChanged();
            }
        }

        public string Stationid_dmi
        {
            get
            {
                return _stationid_dmi;
            }
            set
            {
                _stationid_dmi = value;
                RaisePropertyChanged();
            }
        }

        public string StationType
        {
            get
            {
                return _stationType;
            }
            set
            {
                _stationType = value;
                RaisePropertyChanged();
            }
        }

        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                _country = value;
                RaisePropertyChanged();
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }
        
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

        public string StationOwner
        {
            get
            {
                return _stationOwner;
            }
            set
            {
                _stationOwner = value;
                RaisePropertyChanged();
            }
        }

        public string Wgs_lat
        {
            get
            {
                return _wgs_lat;
            }
            set
            {
                _wgs_lat = value;
                RaisePropertyChanged();
            }
        }

        public string Wgs_long
        {
            get
            {
                return _wgs_long;
            }
            set
            {
                _wgs_long = value;
                RaisePropertyChanged();
            }
        }

        public string Hha
        {
            get
            {
                return _hha;
            }
            set
            {
                _hha = value;
                RaisePropertyChanged();
            }
        }

        public string Hhp
        {
            get
            {
                return _hhp;
            }
            set
            {
                _hhp = value;
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

        public CreateStationInformationDialogViewModel()
        {
            StationTypeOptions = new ObservableCollection<string>(SharedData.StationTypeDisplayTextMap.Values);
            StatusOptions = new ObservableCollection<string>(SharedData.StatusDisplayTextMap.Values);
            CountryOptions = new ObservableCollection<string>(SharedData.CountryDisplayTextMap.Values);
            StationOwnerOptions = new ObservableCollection<string>(SharedData.StationOwnerDisplayTextMap.Values);

            DateFrom = DateTime.UtcNow.AsDateString();
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

            StationName = StationName.NullifyIfEmpty();
            Stationid_dmi = Stationid_dmi.NullifyIfEmpty();
            StationType = StationType.NullifyIfEmpty();
            Country = Country.NullifyIfEmpty();
            Status = Status.NullifyIfEmpty();
            DateFrom = DateFrom.NullifyIfEmpty();
            DateTo = DateTo.NullifyIfEmpty();
            StationOwner = StationOwner.NullifyIfEmpty();
            Wgs_lat = Wgs_lat.NullifyIfEmpty();
            Wgs_long = Wgs_long.NullifyIfEmpty();
            Hha = Hha.NullifyIfEmpty();
            Hhp = Hhp.NullifyIfEmpty();

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
                        case "StationName":
                            {
                                if (string.IsNullOrEmpty(StationName))
                                {
                                    errorMessage = "Required";
                                }
                                else if (StationName.Length > 500)
                                {
                                    errorMessage = "Station Name cannot exceed 500 characters";
                                }
                                break;
                            }
                        case "Stationid_dmi":
                            {
                                if (string.IsNullOrEmpty(Stationid_dmi))
                                {
                                    errorMessage = "Required";
                                }
                                else if (!regexStationId.IsMatch(Stationid_dmi))
                                {
                                    errorMessage = "Invalid station ID";
                                }
                                break;
                            }
                        case "StationType":
                            {
                                if (string.IsNullOrEmpty(StationType))
                                {
                                    errorMessage = "Required";
                                }
                                break;
                            }
                        case "Country":
                            {
                                if (string.IsNullOrEmpty(Country))
                                {
                                    errorMessage = "Required";
                                }
                                break;
                            }
                        case "Status":
                            {
                                if (string.IsNullOrEmpty(Status))
                                {
                                    errorMessage = "Required";
                                }
                                break;
                            }
                        case "DateFrom":
                            {
                                if (string.IsNullOrEmpty(DateFrom))
                                {
                                    errorMessage = "Required";
                                }
                                else if (!DateFrom.IsProperlyFormattedAsADate())
                                {
                                    errorMessage = "Format must be yyyy-mm-dd";
                                }
                                else if (!DateFrom.TryParsingAsDateTime(out var dateTime))
                                {
                                    errorMessage = "Must be a valid date";
                                }

                                break;
                            }
                        case "DateTo":
                            {
                                if (!string.IsNullOrEmpty(DateTo))
                                {
                                    if (!DateTo.IsProperlyFormattedAsADate())
                                    {
                                        errorMessage = "Format must be yyyy-MM-dd";
                                    }
                                    else if (!DateTo.TryParsingAsDateTime(out var dateTime))
                                    {
                                        errorMessage = "Must be a valid date";
                                    }
                                }

                                break;
                            }
                        case "StationOwner":
                            {
                                if (string.IsNullOrEmpty(StationOwner))
                                {
                                    errorMessage = "Required";
                                }
                                break;
                            }
                        case "Wgs_lat":
                            {
                                if (string.IsNullOrEmpty(Wgs_lat))
                                {
                                    errorMessage = "Required";
                                }
                                else if (!double.TryParse(Wgs_lat, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
                                {
                                    errorMessage = "Must be a valid number (use . as decimal separator)";
                                }
                                else if (value < -90.0 || value > 90.0)
                                {
                                    errorMessage = "Must be a number between -90 and 90";
                                }
                                break;
                            }
                        case "Wgs_long":
                            {
                                if (string.IsNullOrEmpty(Wgs_long))
                                {
                                    errorMessage = "Required";
                                }
                                else if (!double.TryParse(Wgs_long, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
                                {
                                    errorMessage = "Must be a valid number (use . as decimal separator)";
                                }
                                else if (value < -180.0 || value > 180.0)
                                {
                                    errorMessage = "Must be a number between -180 and 180";
                                }
                                break;
                            }
                        case "Hha":
                            {
                                if (!string.IsNullOrEmpty(Hha))
                                {
                                    if (!double.TryParse(Hha, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
                                    {
                                        errorMessage = "Must be a valid number (use . as decimal separator)";
                                    }
                                    else if (value < 0 || value > 10000)
                                    {
                                        errorMessage = "Must be a number between 0 and 10000";
                                    }
                                }
                                break;
                            }
                        case "Hhp":
                            {
                                if (!string.IsNullOrEmpty(Hhp))
                                {
                                    if (!double.TryParse(Hhp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
                                    {
                                        errorMessage = "Must be a valid number (use . as decimal separator)";
                                    }
                                    else if (value < 0 || value > 10000)
                                    {
                                        errorMessage = "Must be a number between 0 and 10000";
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
                        new ValidationError {PropertyName = "StationName"},
                        new ValidationError {PropertyName = "Stationid_dmi"},
                        new ValidationError {PropertyName = "StationType"},
                        new ValidationError {PropertyName = "Country"},
                        new ValidationError {PropertyName = "Status"},
                        new ValidationError {PropertyName = "DateFrom"},
                        new ValidationError {PropertyName = "DateTo"},
                        new ValidationError {PropertyName = "StationOwner"},
                        new ValidationError {PropertyName = "Wgs_lat"},
                        new ValidationError {PropertyName = "Wgs_long"},
                        new ValidationError {PropertyName = "Hha"},
                        new ValidationError {PropertyName = "Hhp"},
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
            RaisePropertyChanged("StationName");
            RaisePropertyChanged("Stationid_dmi");
            RaisePropertyChanged("StationType");
            RaisePropertyChanged("Country");
            RaisePropertyChanged("Status");
            RaisePropertyChanged("DateFrom");
            RaisePropertyChanged("DateTo");
            RaisePropertyChanged("StationOwner");
            RaisePropertyChanged("Wgs_lat");
            RaisePropertyChanged("Wgs_long");
            RaisePropertyChanged("Hha");
            RaisePropertyChanged("Hhp");
        }

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }
    }
}
