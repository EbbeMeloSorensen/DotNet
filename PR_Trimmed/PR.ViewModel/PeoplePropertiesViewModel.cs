using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.UI.Utils;
using PR.Application;
using PR.Persistence;
using PR.Domain.Entities.PR;
using PR.Domain.BusinessRules.PR;

namespace PR.ViewModel;

public class PeoplePropertiesViewModel : ViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObjectCollection<Person> _people;
    private BusinessRuleCatalog _businessRuleCatalog;
    private Dictionary<string, string> _errors;

    public Person OriginalSharedValues { get; private set; }
    public Person SharedValues { get; }

    private bool _isVisible;

    private AsyncCommand _applyChangesCommand;

    public event EventHandler<PeopleEventArgs> PeopleUpdated;

    public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

    public string SharedFirstName
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

    public string SharedSurname
    {
        get => SharedValues.Surname ?? "";
        set
        {
            SharedValues.Surname = value;
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
        ObjectCollection<Person> people)
    {
        SharedValues = new Person();
        UnitOfWorkFactory = unitOfWorkFactory;
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

        SharedFirstName = people.All(p => p.FirstName == people.First().FirstName)
            ? people.First().FirstName
            : string.Empty;

        SharedSurname = (people.All(p => p.Surname == people.First().Surname)
            ? people.First().Surname
            : string.Empty) ?? string.Empty;

        OriginalSharedValues = new Person
        {
            FirstName = SharedFirstName,
            Surname = SharedSurname
        };

        ApplyChangesCommand.RaiseCanExecuteChanged();
    }

    private async Task ApplyChanges()
    {
        UpdateState(StateOfView.Updated);

        if (!string.IsNullOrEmpty(Error))
        {
            return;
        }

        var updatedPeople = _people.Objects.Select(p => new Person
        {
            ID = p.ID,
            Superseded = p.Superseded,
            FirstName = SharedFirstName != OriginalSharedValues.FirstName ? SharedFirstName : p.FirstName,
            Surname = SharedSurname != OriginalSharedValues.Surname ? SharedSurname : p.Surname,
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

        return
            SharedFirstName != OriginalSharedValues.FirstName ||
            SharedSurname != OriginalSharedValues.Surname;
    }

    public string this[string columnName]
    {
        get
        {
            var errorMessage = string.Empty;

            return errorMessage;
        }
    }

    public string Error => null; // Not used

    private void RaisePropertyChanges()
    {
        RaisePropertyChanged(nameof(SharedFirstName));
        RaisePropertyChanged(nameof(SharedSurname));
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

        //_errors = _businessRuleCatalog.ValidateAtomic(SharedValues);
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