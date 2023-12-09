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

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

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

                //if (_state == StateOfView.Updated)
                //{
                //    switch (columnName)
                //    {
                //        case "Description":
                //        {
                //            if (Description.Length > 10 /*255*/)
                //            {
                //                errorMessage = "Description cannot exceed 255 characters";
                //            }

                //            break;
                //        }
                //    }
                //}

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
                        //new ValidationError {PropertyName = "Description"},
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

        private void UpdateState(StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }

        private void RaisePropertyChanges()
        {
            //RaisePropertyChanged("Description");
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
