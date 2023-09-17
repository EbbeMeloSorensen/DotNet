using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using Craft.UI.Utils;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;
using Craft.Utils;
using System;
using C2IEDM.Application;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Collections.Generic;

namespace C2IEDM.ViewModel;

public class ObservingFacilitiesDetailsViewModel : ViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private ObjectCollection<ObservingFacility> _observingFacilities;

    private string _originalSharedName;
    private DateTime? _originalSharedDateEstablished;
    private DateTime? _originalSharedDateClosed;

    private string _sharedName;
    private DateTime? _sharedDateEstablished;
    private DateTime? _sharedDateClosed;

    private bool _isVisible;

    private RelayCommand _applyChangesCommand;

    public event EventHandler<ObservingFacilitiesEventArgs> ObservingFacilitiesUpdated;

    public string SharedName
    {
        get { return _sharedName; }
        set
        {
            _sharedName = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public DateTime? SharedDateEstablished
    {
        get { return _sharedDateEstablished; }
        set
        {
            _sharedDateEstablished = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public DateTime? SharedDateClosed
    {
        get { return _sharedDateClosed; }
        set
        {
            _sharedDateClosed = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsVisible
    {
        get { return _isVisible; }
        set
        {
            _isVisible = value;
            RaisePropertyChanged();
        }
    }

    public RelayCommand ApplyChangesCommand
    {
        get { return _applyChangesCommand ?? (_applyChangesCommand = new RelayCommand(ApplyChanges, CanApplyChanges)); }
    }

    public ObservingFacilitiesDetailsViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        ObjectCollection<ObservingFacility> observingFacilities)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _observingFacilities = observingFacilities;

        _observingFacilities.PropertyChanged += Initialize;
    }

    private void Initialize(object sender, PropertyChangedEventArgs e)
    {
        _state = StateOfView.Initial;
        var temp = sender as ObjectCollection<ObservingFacility>;

        var firstObservingFacility = temp?.Objects.FirstOrDefault();

        if (firstObservingFacility == null)
        {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        SharedName = temp.Objects.All(_ => _.Name == firstObservingFacility.Name)
            ? firstObservingFacility.Name
            : null;

        SharedDateEstablished = temp.Objects.All(_ => _.DateEstablished == firstObservingFacility.DateEstablished)
            ? firstObservingFacility.DateEstablished
            : null;

        SharedDateClosed = temp.Objects.All(_ => _.DateClosed == firstObservingFacility.DateClosed)
            ? firstObservingFacility.DateClosed
            : null;

        _originalSharedName = SharedName;
        _originalSharedDateEstablished = SharedDateEstablished;
        _originalSharedDateClosed = SharedDateClosed;

        ApplyChangesCommand.RaiseCanExecuteChanged();
    }

    private void ApplyChanges()
    {
        UpdateState(StateOfView.Updated);

        Error = string.Join("",
            ValidationMessages.Select(e => e.ErrorMessage).ToArray());

        if (!string.IsNullOrEmpty(Error))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var updatedObservingFacilities = _observingFacilities.Objects.Select(_ => new ObservingFacility(_.ObjectId, now)
        {
            Id = _.Id,
            Name = SharedName != _originalSharedName ? SharedName : _.Name,
            DateEstablished = SharedDateEstablished != _originalSharedDateEstablished
                ? new DateTime(
                    SharedDateEstablished.Value.Year,
                    SharedDateEstablished.Value.Month,
                    SharedDateEstablished.Value.Day,
                    0, 0, 0, DateTimeKind.Utc)
                : _.DateEstablished,
            DateClosed = SharedDateClosed != _originalSharedDateClosed
                ? new DateTime(
                    SharedDateClosed.Value.Year,
                    SharedDateClosed.Value.Month,
                    SharedDateClosed.Value.Day,
                    0, 0, 0, DateTimeKind.Utc)
                : _.DateClosed,
        }).ToList();

        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            unitOfWork.ObservingFacilities.UpdateRange(updatedObservingFacilities);
            unitOfWork.Complete();
        }

        OnObservingFacilitiesUpdated(updatedObservingFacilities);
    }

    private bool CanApplyChanges()
    {
        return
            SharedName != _originalSharedName ||
            SharedDateEstablished != _originalSharedDateEstablished ||
            SharedDateClosed != _originalSharedDateClosed;
    }

    public ObservableCollection<ValidationError> ValidationMessages
    {
        get
        {
            if (_validationMessages == null)
            {
                _validationMessages = new ObservableCollection<ValidationError>
                {
                    new ValidationError {PropertyName = "SharedName"},
                    new ValidationError {PropertyName = "SharedDateEstablished"},
                    new ValidationError {PropertyName = "SharedDateClosed"}
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
                    case "SharedName":
                        {
                            if (string.IsNullOrEmpty(SharedName))
                            {
                                if (_observingFacilities.Objects.Count() == 1)
                                {
                                    errorMessage = "Name is required";
                                }
                            }
                            else if (SharedName.Length > 127)
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
        RaisePropertyChanged("SharedName");
        RaisePropertyChanged("SharedDateEstablished");
        RaisePropertyChanged("SharedDateClosed");
    }

    private void UpdateState(StateOfView state)
    {
        _state = state;
        RaisePropertyChanges();
    }

    private void OnObservingFacilitiesUpdated(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
        var handler = ObservingFacilitiesUpdated;

        // Event will be null if there are no subscribers
        if (handler != null)
        {
            handler(this, new ObservingFacilitiesEventArgs(observingFacilities));
        }
    }
}