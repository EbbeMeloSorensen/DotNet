using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.UI.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Domain.BusinessRules.PR;
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
        private BusinessRuleCatalog _businessRuleCatalog;
        public Person Person { get; }

        private string _dateRangeError;
        private bool _displayDateRangeError;

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
            BusinessRuleCatalog businessRuleCatalog)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _businessRuleCatalog = businessRuleCatalog;
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

            if (_errors.Values.Any(_ => !string.IsNullOrEmpty(_)))
            {
                return;
            }

            //FirstName = FirstName.NullifyIfEmpty();
            //Surname = Surname.NullifyIfEmpty();
            //Nickname = Nickname.NullifyIfEmpty();
            //Address = Address.NullifyIfEmpty();
            //ZipCode = ZipCode.NullifyIfEmpty();
            //City = City.NullifyIfEmpty();
            //Category = Category.NullifyIfEmpty();

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                await unitOfWork.People.Add(Person);
                unitOfWork.Complete();
            }

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
                string? error;
                DateRangeError = "";

                if (columnName == "Start" ||
                    columnName == "End")
                {
                    _errors.TryGetValue("DateRange", out error);
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

            _errors = _businessRuleCatalog.Validate(Person);

            if (_errors.ContainsKey("DateRange"))
            {
                DateRangeError = _errors["DateRange"];
            }

            RaisePropertyChanges();
        }
    }
}
