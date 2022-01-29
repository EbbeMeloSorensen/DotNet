using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;

namespace DD.ViewModel
{
    // Todo: Placer de her hjælpeklasser et mere hensigtsmæssigt sted

    public enum StateOfView
    {
        Initial,
        Updated
    }

    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static class StringExtensions
    {
        public static string NullifyIfEmpty(this string s)
        {
            if (s != null && s.Trim().Equals(string.Empty))
            {
                return null;
            }

            return s;
        }
    }

    public class CreateCreatureTypeDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private string _name;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
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

        private void OK(object parameter)
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            Name = Name.NullifyIfEmpty();

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
                        case "Name":
                            {
                                if (string.IsNullOrEmpty(Name))
                                {
                                    errorMessage = "Name is required";
                                }
                                else if (Name.Length > 127)
                                {
                                    errorMessage = "Name cannot exceed 127 characters";
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
                        new ValidationError {PropertyName = "Name"},
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
            RaisePropertyChanged("Name");
        }

        private void UpdateState(
            StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }
    }
}
