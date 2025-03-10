using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using PR.Domain.BusinessRules.PR;
using PR.Domain.Entities.PR;

namespace PR.ViewModel
{
    public class CreatePersonDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private string _firstName;
        private string _surname;
        private string _nickname;
        private string _address;
        private string _zipCode;
        private string _city;
        private DateTime? _birthday;
        private string _category;
        private DateTime? _start;
        private DateTime? _end;

        private string _dateRangeError;
        private bool _displayDateRangeError;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        private BusinessRuleCatalog _businessRuleCatalog;

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

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged();
            }
        }

        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                RaisePropertyChanged();
            }
        }

        public string Nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                RaisePropertyChanged();
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }

        public string ZipCode
        {
            get { return _zipCode; }
            set
            {
                _zipCode = value;
                RaisePropertyChanged();
            }
        }

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged();
            }
        }

        public DateTime? Birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                RaisePropertyChanged();
            }
        }

        public DateTime? Start
        {
            get { return _start; }
            set
            {
                _start = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(End));
            }
        }

        public DateTime? End
        {
            get { return _end; }
            set
            {
                _end = value;
                RaisePropertyChanged(nameof(Start));
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
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

        public CreatePersonDialogViewModel(
            BusinessRuleCatalog businessRuleCatalog)
        {
            _businessRuleCatalog = businessRuleCatalog;
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
                var errorMessage = string.Empty;

                if (_state == StateOfView.Updated)
                {
                    DateRangeError = string.Empty;

                    // Experimental
                    var person = new Person
                    {
                        FirstName = FirstName.NullifyIfEmpty(),
                        Surname = Surname.NullifyIfEmpty(),
                        Nickname = Nickname.NullifyIfEmpty(),
                        Address = Address.NullifyIfEmpty(),
                        ZipCode = ZipCode.NullifyIfEmpty(),
                        City = City.NullifyIfEmpty(),
                        Birthday = Birthday,
                        Category = Category,
                        Start = Start ?? DateTime.UtcNow,
                        End = End ?? new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                    };

                    var temp = _businessRuleCatalog.Validate(person);
                    // end


                    var now = DateTime.UtcNow;

                    switch (columnName)
                    {
                        case "FirstName":
                            {
                                if (string.IsNullOrEmpty(FirstName))
                                {
                                    errorMessage = "First name is required";
                                }
                                else if (FirstName.Length > 127)
                                {
                                    errorMessage = "First name cannot exceed 127 characters";
                                }

                                break;
                            }
                        case "Surname":
                            {
                                if (Surname != null && Surname.Length > 255)
                                {
                                    errorMessage = "Surname cannot exceed 255 characters";
                                }

                                break;
                            }
                        case "Nickname":
                            {
                                if (Nickname != null && Nickname.Length > 127)
                                {
                                    errorMessage = "Nickname cannot exceed 127 characters";
                                }

                                break;
                            }
                        case "Address":
                            {
                                if (Address != null && Address.Length > 511)
                                {
                                    errorMessage = "Address cannot exceed 511 characters";
                                }

                                break;
                            }
                        case "ZipCode":
                            {
                                if (ZipCode != null && ZipCode.Length > 127)
                                {
                                    errorMessage = "Zip code cannot exceed 127 characters";
                                }

                                break;
                            }
                        case "City":
                            {
                                if (City != null && City.Length > 255)
                                {
                                    errorMessage = "City cannot exceed 255 characters";
                                }

                                break;
                            }
                        case "Category":
                            {
                                if (Category != null && Category.Length > 127)
                                {
                                    errorMessage = "Category cannot exceed 127 characters";
                                }

                                break;
                            }
                        case "Start":
                        case "End":
                        {
                            if ((Start != null && Start >= now) ||
                                (End != null && End >= now))
                            {
                                errorMessage = "These dates cannot be in the future";
                                DateRangeError = errorMessage;
                            }
                            else if (End != null && Start == null)
                            {
                                errorMessage = "When entering an end date, a start date is also required";
                                DateRangeError = errorMessage;
                            }
                            else if (Start != null && End != null && Start >= End)
                            {
                                errorMessage = "Start date must be earlier than End date";
                                DateRangeError = errorMessage;
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

        public ObservableCollection<ValidationError> ValidationMessages
        {
            get
            {
                if (_validationMessages == null)
                {
                    _validationMessages = new ObservableCollection<ValidationError>
                    {
                        new ValidationError {PropertyName = "FirstName"},
                        new ValidationError {PropertyName = "Surname"},
                        new ValidationError {PropertyName = "Nickname"},
                        new ValidationError {PropertyName = "Address"},
                        new ValidationError {PropertyName = "ZipCode"},
                        new ValidationError {PropertyName = "City"},
                        new ValidationError {PropertyName = "Birthday"},
                        new ValidationError {PropertyName = "Category"},
                        new ValidationError {PropertyName = "Start"},
                        new ValidationError {PropertyName = "End"}
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
            RaisePropertyChanges();
        }
    }
}
