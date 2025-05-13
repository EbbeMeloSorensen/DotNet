using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Craft.Domain;
using GalaSoft.MvvmLight.Command;
using Craft.UI.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Domain.Entities.PR;
using PR.Persistence;

namespace PR.ViewModel
{
    public class CreatePersonDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private static readonly DateTime _maxDateTime = new(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        private StateOfView _state;
        private Dictionary<string, string> _errors;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IBusinessRuleCatalog _businessRuleCatalog;

        public Person Person { get; }

        private IEnumerable<Tuple<DateTime, DateTime>> _occupiedDateRanges;

        private string _latitude;
        private string _longitude;
        private string _dateRangeError;
        private bool _displayDateRangeError;

        private string _generalError;
        private bool _displayGeneralError;

        private AsyncCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public string FirstName
        {
            get => Person.FirstName;
            set
            {
                Person.FirstName = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Surname
        {
            get => Person.Surname ?? "";
            set
            {
                Person.Surname = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Nickname
        {
            get => Person.Nickname ?? "";
            set
            {
                Person.Nickname = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Address
        {
            get => Person.Address ?? "";
            set
            {
                Person.Address = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string ZipCode
        {
            get => Person.ZipCode ?? "";
            set
            {
                Person.ZipCode = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string City
        {
            get => Person.City ?? "";
            set
            {
                Person.City = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get => Person.Category ?? "";
            set
            {
                Person.Category = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a double
        // So the view model has to check that before we involve the business rule catalog
        public string Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a double
        // So the view model has to check that before we involve the business rule catalog
        public string Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public DateTime? Birthday
        {
            get => Person.Birthday;
            set
            {
                Person.Birthday = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public DateTime? Start
        {
            get
            {
                if (Person.Start == default)
                {
                    return null;
                }

                return Person.Start;
            }
            set
            {
                Person.Start = value ?? default;
                Validate();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(End));
            }
        }

        public DateTime? End
        {
            get
            {
                if (Person.End == _maxDateTime)
                {
                    return null;
                }

                return Person.End;
            }
            set
            {
                Person.End = value ?? new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                Validate();
                RaisePropertyChanged(nameof(Start));
                RaisePropertyChanged();
            }
        }

        public string DateRangeError
        {
            get => _dateRangeError;
            set
            {
                _dateRangeError = value;
                DisplayDateRangeError = !string.IsNullOrEmpty(_dateRangeError);
                RaisePropertyChanged();
            }
        }

        public bool DisplayDateRangeError
        {
            get => _displayDateRangeError;
            set
            {
                _displayDateRangeError = value;
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
            get { return _okCommand ?? (_okCommand = new AsyncCommand<object>(OK, CanOK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel, CanCancel)); }
        }

        public CreatePersonDialogViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IBusinessRuleCatalog businessRuleCatalog,
            IEnumerable<Tuple<DateTime, DateTime>> occupiedDateRanges = null)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _businessRuleCatalog = businessRuleCatalog;
            _occupiedDateRanges = occupiedDateRanges;

            _errors = new Dictionary<string, string>();

            Person = new Person
            {
                Start = DateTime.UtcNow.Date,
                End = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };
        }

        private async Task OK(object parameter)
        {
            UpdateState(StateOfView.Updated);

            //if (_errors.Values.Any(_ => !string.IsNullOrEmpty(_)))
            if (_errors.Values.Any())
            {
                return;
            }

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    await unitOfWork.People.Add(Person);
                    unitOfWork.Complete();
                }

                CloseDialogWithResult(parameter as Window, DialogResult.OK);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    GeneralError = e.InnerException.Message;
                }
                else
                {
                    GeneralError = e.Message;
                }
            }
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
                // In the basic pattern, validation is done here, but in this design
                // validation is done as soon as a property is updated so that the collection
                // of errors is ready when this indexer is called

                if (_state == StateOfView.Initial)
                {
                    return string.Empty;
                }

                string? error;

                if (columnName == "Start" ||
                    columnName == "End")
                {
                    _errors.TryGetValue("DateRange", out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        _errors.TryGetValue("NoOverlappingValidTimeIntervals", out error);
                    }
                }
                else
                {
                    _errors.TryGetValue(columnName, out error);
                }

                return error ?? "";
            }
        }

        public string Error => null; // Not used

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged(nameof(FirstName));
            RaisePropertyChanged(nameof(Surname));
            RaisePropertyChanged(nameof(Nickname));
            RaisePropertyChanged(nameof(Address));
            RaisePropertyChanged(nameof(ZipCode));
            RaisePropertyChanged(nameof(City));
            RaisePropertyChanged(nameof(Birthday));
            RaisePropertyChanged(nameof(Category));
            RaisePropertyChanged(nameof(Latitude));
            RaisePropertyChanged(nameof(Longitude));
            RaisePropertyChanged(nameof(Start));
            RaisePropertyChanged(nameof(End));
        }

        private void UpdateState(
            StateOfView state)
        {
            _state = state;
            Validate();
            RaisePropertyChanges();
        }

        private void Validate()
        {
            if (_state != StateOfView.Updated) return;

            _errors.Clear();
            DateRangeError = "";

            ValidateNumericInput(nameof(Latitude), Latitude, out var latitude);
            ValidateNumericInput(nameof(Longitude), Longitude, out var longitude);

            if (_errors.Any())
            {
                return;
            }

            Person.Latitude = latitude;
            Person.Longitude = longitude;

            _errors = _businessRuleCatalog.ValidateAtomic(Person);

            if (_errors.Any())
            {
                if (_errors.TryGetValue("DateRange", out var error))
                {
                    DateRangeError = error;
                }
            }
            else if (_occupiedDateRanges != null)
            {
                var dateRanges = _occupiedDateRanges
                    .Append(new Tuple<DateTime, DateTime>(Person.Start, Person.End))
                    .OrderBy(_ => _.Item1);

                _errors = _businessRuleCatalog.ValidateCrossEntity(dateRanges);

                if (_errors.TryGetValue("NoOverlappingValidTimeIntervals", out var error))
                {
                    DateRangeError = error;
                }
            }
        }

        private void ValidateNumericInput(
            string propertyName,
            string text,
            out double? value)
        {
            if (string.IsNullOrEmpty(text))
            {
                value = null;
            }
            else if(double.TryParse(
                        text,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture,
                        out var temp))
            {
                value = temp;
            }
            else
            {
                value = double.NaN;
                _errors[propertyName] = "Invalid format";
            }
        }
    }
}
