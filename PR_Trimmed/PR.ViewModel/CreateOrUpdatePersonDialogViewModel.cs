using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.Math;
using Craft.Domain;
using Craft.UI.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Domain.Entities.PR;
using PR.Persistence;
using PR.Persistence.Versioned;
using PR.Domain;

namespace PR.ViewModel
{
    public enum CreateOrUpdatePersonDialogViewModelMode
    {
        Create,
        Update
    }

    public class CreateOrUpdatePersonDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private static readonly DateTime _maxDateTime = new(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        private CreateOrUpdatePersonDialogViewModelMode _mode;
        private StateOfView _state;
        private Dictionary<string, string> _errors;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IBusinessRuleCatalog _businessRuleCatalog;

        public Person Person { get; }

        private IEnumerable<Person> _otherVariants;

        private string _latitude;
        private string _longitude;
        private string _startTime;
        private string _endTime;
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

        public DateTime? StartDate
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
                RaisePropertyChanged(nameof(EndDate));
                RaisePropertyChanged(nameof(StartTime));
                RaisePropertyChanged(nameof(EndTime));
            }
        }

        public DateTime? EndDate
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
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(StartTime));
                RaisePropertyChanged(nameof(EndTime));
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a Time
        // So the view model has to check that before we involve the business rule catalog
        public string StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                Validate();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(EndDate));
                RaisePropertyChanged(nameof(EndTime));
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a Time
        // So the view model has to check that before we involve the business rule catalog
        public string EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                Validate();
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(EndDate));
                RaisePropertyChanged(nameof(StartTime));
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

        public CreateOrUpdatePersonDialogViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IBusinessRuleCatalog businessRuleCatalog,
            Person person = null,
            IEnumerable<Person> otherVariants = null)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _businessRuleCatalog = businessRuleCatalog;

            _mode = person == null
                ? CreateOrUpdatePersonDialogViewModelMode.Create
                : CreateOrUpdatePersonDialogViewModelMode.Update;

            _otherVariants = otherVariants;

            _latitude = string.Empty;
            _longitude = string.Empty;
            _startTime = "00:00:00";
            _endTime = string.Empty;

            _errors = new Dictionary<string, string>();
            _dateRangeError = string.Empty;
            _generalError = string.Empty;

            Person = person == null
                ? new Person
                {
                    FirstName = _otherVariants != null && _otherVariants.Any() ? _otherVariants.Last().FirstName : "",
                    Start = DateTime.UtcNow.Date,
                    End = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                }
                : person;
        }

        private async Task OK(object parameter)
        {
            UpdateState(StateOfView.Updated);

            if (_errors.Values.Any())
            {
                return;
            }

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    switch (_mode)
                    {
                        case CreateOrUpdatePersonDialogViewModelMode.Create:

                            if (_otherVariants != null)
                            {
                                _otherVariants.InsertNewVariant(
                                    Person,
                                    out var nonConflictingEntities,
                                    out var coveredEntities,
                                    out var trimmedEntities,
                                    out var newEntities);

                                if (coveredEntities.Any())
                                {
                                    await unitOfWork.People.RemoveRange(coveredEntities);
                                }

                                if (trimmedEntities.Any())
                                {
                                    await unitOfWork.People.UpdateRange(trimmedEntities);
                                }

                                if (newEntities.Any())
                                {
                                    await unitOfWork.People.UpdateRange(trimmedEntities);
                                }
                            }

                            //await unitOfWork.People.Add(Person);

                            break;
                        case CreateOrUpdatePersonDialogViewModelMode.Update:
                            await unitOfWork.People.Correct(Person);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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

                // For these fields, we want to highlight them all if there is a problem with the date range
                if (columnName == nameof(StartDate) ||
                    columnName == nameof(StartTime) ||
                    columnName == nameof(EndDate) ||
                    columnName == nameof(EndTime))
                {
                    _errors.TryGetValue("DateRange", out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        _errors.TryGetValue("ValidTimeIntervals", out error);
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
            RaisePropertyChanged(nameof(StartDate));
            RaisePropertyChanged(nameof(StartTime));
            RaisePropertyChanged(nameof(EndDate));
            RaisePropertyChanged(nameof(EndTime));
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

            // Initially, we ensure that the fields that need to be parsed can be parsed correctly
            if (!Latitude.TryParse(out var latitude, out var error_lat))
            {
                _errors[nameof(Latitude)] = error_lat;
            }

            if (!Longitude.TryParse(out var longitude, out var error_long))
            {
                _errors[nameof(Longitude)] = error_long;
            }

            DateTime startTime = default;
            if (string.IsNullOrEmpty(_startTime))
            {
                DateRangeError = "Start time is required";
                _errors["DateRange"] = DateRangeError;
            }
            else if (!DateTime.TryParseExact(_startTime, "HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal, out startTime))
            {
                DateRangeError = "Time format needs to be HH:mm:ss";
                _errors["DateRange"] = DateRangeError;
            }

            var endTime = EndDate.HasValue
                ? new DateTime()
                : new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(_endTime))
            {
                if (DateTime.TryParseExact(_endTime, "HH:mm:ss", CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal, out var temp))
                {
                    endTime = temp;
                }
                else
                {
                    DateRangeError = "Time format needs to be HH:mm:ss";
                    _errors["DateRange"] = DateRangeError;
                }
            }

            if (_errors.Any())
            {
                return;
            }

            // Then, we set the properties of the Person object and validate it using the business rule catalog
            Person.Latitude = latitude;
            Person.Longitude = longitude;
            Person.Start = Person.Start.Date + startTime.TimeOfDay;
            Person.End = Person.End.Date + endTime.TimeOfDay;

            _errors = _businessRuleCatalog.ValidateAtomic(Person);

            string error;

            if (_errors.Any())
            {
                if (_errors.TryGetValue("DateRange", out error))
                {
                    DateRangeError = error;
                }

                return;
            }

            // If we got this far, then the Person object is valid in itself, but it may conflict with other variants

            // Vi kan ikke bare tage de andre varianter as is - nogle af dem vil måske skulle fjernes,
            // andre af dem vil måske skulle trimmes - og det kan endda ske, at et tidsinterval vil skulle splittes op i 2 nye
            // (hvis det dominante tidsinterval "dækkes" af et eksisterende)

            // Vi skal have en method, der opererer på en collection, modtager en (ny) variant som parameter og returnerer 3 puljer:
            // 1) varianter, der skal slettes (fordi de dækkes af den nye variant)
            // 2) varianter, der skal opdateres (fordi de overlapper med den nye variant)
            // 3) varianter, der skal genereres (fordi en variant deles i 2 stykker af den nye variant)
            // 4) varianter, der skal bibeholdes (fordi de ikke konflikter med den nye variant)

            if (_otherVariants != null)
            {
                _otherVariants.InsertNewVariant(
                Person,
                out var nonConflictingEntities,
                out var coveredEntities,
                out var trimmedEntities,
                out var newEntities);

                var newPotentialEntityCollection = nonConflictingEntities;
                newPotentialEntityCollection.AddRange(trimmedEntities);
                newPotentialEntityCollection.AddRange(newEntities);
                newPotentialEntityCollection.Add(Person);

                newPotentialEntityCollection = newPotentialEntityCollection.OrderBy(_ => _.Start).ToList();
                _errors = _businessRuleCatalog.ValidateCrossEntity(newPotentialEntityCollection);
            }

            if (_errors.TryGetValue("ValidTimeIntervals", out error))
            {
                DateRangeError = error;
            }
            else if(_errors.TryGetValue("BirthdayConsistency", out error))
            {
                DateRangeError = error;
            }
        }
    }
}
