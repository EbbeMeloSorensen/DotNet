using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;

namespace Glossary.ViewModel
{
    public class CreateRecordDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private string _term;
        private string _source;
        private string _category;
        private string _comments;

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public string Term
        {
            get { return _term; }
            set
            {
                _term = value;
                RaisePropertyChanged();
            }
        }

        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
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

        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
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

            Term = Term.NullifyIfEmpty();
            Source = Source.NullifyIfEmpty();
            Category = Category.NullifyIfEmpty();
            Comments = Comments.NullifyIfEmpty();

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
                        case "Term":
                            {
                                if (string.IsNullOrEmpty(Term))
                                {
                                    errorMessage = "Term is required";
                                }
                                else if (Term.Length > 127)
                                {
                                    errorMessage = "Term cannot exceed 127 characters";
                                }

                                break;
                            }
                        case "Source":
                            {
                                if (Source != null && Source.Length > 511)
                                {
                                    errorMessage = "Source cannot exceed 511 characters";
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
                        case "Comments":
                            {
                                if (Comments != null && Comments.Length > 2047)
                                {
                                    errorMessage = "Comments cannot exceed 2047 characters";
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
                        new ValidationError {PropertyName = "Term"},
                        new ValidationError {PropertyName = "Source"},
                        new ValidationError {PropertyName = "Category"},
                        new ValidationError {PropertyName = "Comments"}
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
            RaisePropertyChanged("Term");
            RaisePropertyChanged("Source");
            RaisePropertyChanged("Category");
            RaisePropertyChanged("Comments");
        }

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }
    }
}
