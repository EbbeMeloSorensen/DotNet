using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using PR.Domain.BusinessRules.PR;
using PR.Domain.Entities.PR;

namespace PR.ViewModel
{
    public class CreatePersonDialogViewModelNew : DialogViewModelBase, IDataErrorInfo
    {
        private static readonly DateTime _maxDateTime = new(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        private StateOfView _state;
        private Dictionary<string, string> _errors;

        private BusinessRuleCatalog _businessRuleCatalog;
        private Person _person;

        private string _dateRangeError;
        private bool _displayDateRangeError;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public string FirstName
        {
            get => _person.FirstName;
            set
            {
                _person.FirstName = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Surname
        {
            get => _person.Surname;
            set
            {
                _person.Surname = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Nickname
        {
            get => _person.Nickname;
            set
            {
                _person.Nickname = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Address
        {
            get => _person.Address;
            set
            {
                _person.Address = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string ZipCode
        {
            get => _person.ZipCode;
            set
            {
                _person.ZipCode = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string City
        {
            get => _person.City;
            set
            {
                _person.City = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get => _person.Category;
            set
            {
                _person.Category = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public DateTime? Birthday
        {
            get => _person.Birthday;
            set
            {
                _person.Birthday = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public DateTime? Start
        {
            get
            {
                if (_person.Start == default)
                {
                    return null;
                }

                return _person.Start;
            }
            set
            {
                _person.Start = value ?? default;
                Validate();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(End));
            }
        }

        public DateTime? End
        {
            get
            {
                if (_person.End == _maxDateTime)
                {
                    return null;
                }

                return _person.End;
            }
            set
            {
                _person.End = value ?? new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                Validate();
                RaisePropertyChanged(nameof(Start));
                RaisePropertyChanged();
            }
        }

        public string DateRangeError
        {
            get { return _dateRangeError; }
            set
            {
                _dateRangeError = value;
                DisplayDateRangeError = !string.IsNullOrEmpty(_dateRangeError);
                RaisePropertyChanged();
            }
        }

        public bool DisplayDateRangeError
        {
            get { return _displayDateRangeError; }
            set
            {
                _displayDateRangeError = value;
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

        public CreatePersonDialogViewModelNew(
            BusinessRuleCatalog businessRuleCatalog)
        {
            _businessRuleCatalog = businessRuleCatalog;
            _errors = new Dictionary<string, string>();

            _person = new Person
            {
                Start = DateTime.UtcNow.Date,
                End = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };
        }

        private void OK(object parameter)
        {
            UpdateState(StateOfView.Updated);

            if (_errors.Values.Any(_ => !string.IsNullOrEmpty(_)))
            {
                return;
            }

            FirstName = FirstName.NullifyIfEmpty();
            Surname = Surname.NullifyIfEmpty();
            Nickname = Nickname.NullifyIfEmpty();
            Address = Address.NullifyIfEmpty();
            ZipCode = ZipCode.NullifyIfEmpty();
            City = City.NullifyIfEmpty();
            Category = Category.NullifyIfEmpty();

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
                string error = null;

                if (columnName == "Start" ||
                    columnName == "End")
                {
                    _errors.TryGetValue("DateRange", out error);
                }
                else
                {
                    _errors.TryGetValue(columnName, out error);
                }

                return error;
            }
        }

        public string Error => null; // Not used

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged("FirstName");
            RaisePropertyChanged("Surname");
            RaisePropertyChanged("Nickname");
            RaisePropertyChanged("Address");
            RaisePropertyChanged("ZipCode");
            RaisePropertyChanged("City");
            RaisePropertyChanged("Birthday");
            RaisePropertyChanged("Category");
            RaisePropertyChanged("Start");
            RaisePropertyChanged("End");
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

            _errors = _businessRuleCatalog.Validate(_person);

            if (_errors.ContainsKey("DateRange"))
            {
                DateRangeError = _errors["DateRange"];
            }

            RaisePropertyChanges();
        }
    }
}
