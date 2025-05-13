using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Craft.Domain;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.UI.Utils;
using PR.Application;
using PR.Persistence;
using PR.Domain.Entities.PR;

namespace PR.ViewModel;

public class PeoplePropertiesViewModel : ViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private IBusinessRuleCatalog _businessRuleCatalog;
    private ObjectCollection<Person> _people;
    private Dictionary<string, string> _errors;

    public Person OriginalSharedValues { get; private set; }
    public Person SharedValues { get; }

    private bool _isVisible;

    private AsyncCommand _applyChangesCommand;

    public event EventHandler<PeopleEventArgs> PeopleUpdated;

    public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

    public string FirstName
    {
        get => SharedValues.FirstName;
        set
        {
            SharedValues.FirstName = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string Surname
    {
        get => SharedValues.Surname ?? "";
        set
        {
            SharedValues.Surname = string.IsNullOrEmpty(value) ? null : value;
            Validate();
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

    public AsyncCommand ApplyChangesCommand
    {
        get { return _applyChangesCommand ?? (_applyChangesCommand = new AsyncCommand(ApplyChanges, CanApplyChanges)); }
    }

    public PeoplePropertiesViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBusinessRuleCatalog businessRuleCatalog,
        ObjectCollection<Person> people)
    {
        UnitOfWorkFactory = unitOfWorkFactory;
        _businessRuleCatalog = businessRuleCatalog;
        SharedValues = new Person();
        _errors = new Dictionary<string, string>();
        _people = people;
        _people.PropertyChanged += Initialize;
    }

    private void Initialize(
        object sender,
        PropertyChangedEventArgs e)
    {
        _state = StateOfView.Initial;

        if (sender is not ObjectCollection<Person> objectCollection) return;

        var people = objectCollection.Objects.ToList();

        if (!people.Any())
        {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        FirstName = people.All(p => p.FirstName == people.First().FirstName)
            ? people.First().FirstName
            : string.Empty;

        Surname = (people.All(p => p.Surname == people.First().Surname)
            ? people.First().Surname
            : string.Empty) ?? string.Empty;

        OriginalSharedValues = new Person
        {
            FirstName = FirstName,
            Surname = Surname
        };

        ApplyChangesCommand.RaiseCanExecuteChanged();
    }

    private async Task ApplyChanges()
    {
        UpdateState(StateOfView.Updated);

        if (_errors.Any())
        {
            return;
        }

        var updatedPeople = _people.Objects.Select(p => new Person
        {
            ID = p.ID,
            Superseded = p.Superseded,
            FirstName = FirstName != OriginalSharedValues.FirstName ? FirstName : p.FirstName,
            Surname = Surname != OriginalSharedValues.Surname ? Surname : p.Surname,
        }).ToList();

        using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
        {
            await unitOfWork.People.UpdateRange(updatedPeople);
            unitOfWork.Complete();
        }

        OnPeopleUpdated(updatedPeople);
    }

    private bool CanApplyChanges()
    {
        if (OriginalSharedValues == null)
        {
            return false;
        }

        if (_state == StateOfView.Updated && _errors.Any())
        {
            return false;
        }

        return
            FirstName != OriginalSharedValues.FirstName ||
            Surname != OriginalSharedValues.Surname;
    }

    public string this[string columnName]
    {
        get
        {
            if (_state == StateOfView.Initial)
            {
                return string.Empty;
            }

            _errors.TryGetValue(columnName, out var error);

            return error ?? "";
        }
    }

    public string Error => null; // Not used

    private void RaisePropertyChanges()
    {
        RaisePropertyChanged(nameof(FirstName));
        RaisePropertyChanged(nameof(Surname));
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

        //ValidateNumericInput(nameof(Latitude), Latitude, out var latitude);
        //ValidateNumericInput(nameof(Longitude), Longitude, out var longitude);

        if (_errors.Any())
        {
            return;
        }

        //SharedValues.Latitude = latitude;
        //SharedValues.Longitude = longitude;
        SharedValues.Start = DateTime.UtcNow;
        SharedValues.End = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        _errors = _businessRuleCatalog.ValidateAtomic(SharedValues);
    }

    private void OnPeopleUpdated(
        IEnumerable<Person> people)
    {
        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
        var handler = PeopleUpdated;

        // Event will be null if there are no subscribers
        if (handler != null)
        {
            handler(this, new PeopleEventArgs(people));
        }
    }
}