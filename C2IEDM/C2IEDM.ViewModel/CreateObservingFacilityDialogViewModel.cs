using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.UI.Utils;
using Craft.ViewModels.Dialogs;

namespace C2IEDM.ViewModel;

public class CreateObservingFacilityDialogViewModel : DialogViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private string _name;
    private DateTime? _dateEstablished;
    private DateTime? _dateClosed;
    private double _latitude;
    private double _longitude;
    
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

    public DateTime? DateEstablished
    {
        get { return _dateEstablished; }
        set
        {
            _dateEstablished = value;
            RaisePropertyChanged();
        }
    }

    public DateTime? DateClosed
    {
        get { return _dateClosed; }
        set
        {
            _dateClosed = value;
            RaisePropertyChanged();
        }
    }

    public double Latitude
    {
        get { return _latitude; }
        set
        {
            _latitude = value;
            RaisePropertyChanged();
        }
    }

    public double Longitude
    {
        get { return _longitude; }
        set
        {
            _longitude = value;
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

        Error = string.Join("", ValidationMessages.Select(e => e.ErrorMessage).ToArray());

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
                    case "Latitude":
                        {

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
                    new ValidationError {PropertyName = "DateEstablished"},
                    new ValidationError {PropertyName = "DateClosed"},
                    new ValidationError {PropertyName = "Latitude"},
                    new ValidationError {PropertyName = "Longitude"}
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

    public CreateObservingFacilityDialogViewModel(
        Point mousePositionWorld)
    {
        Latitude = Math.Round(mousePositionWorld.X, 4);
        Longitude = Math.Round(mousePositionWorld.Y, 4);
    }

    private void RaisePropertyChanges()
    {
        RaisePropertyChanged("Name");
        RaisePropertyChanged("DateEstablished");
        RaisePropertyChanged("DateClosed");
        RaisePropertyChanged("Latitude");
        RaisePropertyChanged("Longitude");
    }

    private void UpdateState(StateOfView state)
    {
        _state = state;
        RaisePropertyChanges();
    }
}