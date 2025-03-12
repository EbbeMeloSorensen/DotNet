using System.Collections.ObjectModel;
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
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private BusinessRuleCatalog _businessRuleCatalog;
        private Person _person;

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
            _person = new Person();
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

            FirstName = FirstName.NullifyIfEmpty();

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

        private ObservableCollection<ValidationError> ValidationMessages
        {
            get
            {
                if (_validationMessages == null)
                {
                    _validationMessages = new ObservableCollection<ValidationError>
                    {
                        new ValidationError {PropertyName = "FirstName"},
                        new ValidationError {PropertyName = "Surname"},
                    };
                }

                return _validationMessages;
            }
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
                        case "FirstName":
                            {
                                if (string.IsNullOrEmpty(FirstName))
                                {
                                    errorMessage = "First name is required";
                                }

                                break;
                            }
                    }
                }

                ValidationMessages
                    .First(e => e.PropertyName == columnName).ErrorMessage = errorMessage;

                return errorMessage;
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
            RaisePropertyChanged("FirstName");
            RaisePropertyChanged("Surname");
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

            var errors = _businessRuleCatalog.Validate(_person);

            //_errors = BusinessRuleCatalog.Validate(_person);
            //OnPropertyChanged(nameof(Name));
            //OnPropertyChanged(nameof(Age));
            //OnPropertyChanged(nameof(Email));
        }
    }
}
