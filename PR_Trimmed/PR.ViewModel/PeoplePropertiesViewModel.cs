using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModel.Utils;
using PR.Application;
using PR.Persistence;
using PR.Domain.Entities.PR;
using PR.Domain.BusinessRules.PR;

namespace PR.ViewModel;

public class PeoplePropertiesViewModel : ViewModelBase, IDataErrorInfo
{
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

    public string SharedNickname
    {
        get => SharedValues.Nickname ?? "";
        set
        {
            SharedValues.Nickname = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedAddress
    {
        get => SharedValues.Address ?? "";
        set
        {
            SharedValues.Address = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedZipCode
    {
        get => SharedValues.ZipCode ?? "";
        set
        {
            SharedValues.ZipCode = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedCity
    {
        get => SharedValues.City ?? "";
        set
        {
            SharedValues.City = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public DateTime? SharedBirthday
    {
        get => SharedValues.Birthday;
        set
        {
            SharedValues.Birthday = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedCategory
    {
        get => SharedValues.Category ?? "";
        set
        {
            SharedValues.Category = value;
            Validate();
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public double? SharedLatitude
    {
        get => SharedValues.Latitude;
        set
        {
            SharedValues.Latitude = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public double? SharedLongitude
    {
        get => SharedValues.Longitude;
        set
        {
            SharedValues.Longitude = value;
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
        var people = sender as ObjectCollection<Person>;

        var firstPerson = people?.Objects.FirstOrDefault();

        if (firstPerson == null)
        {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        SharedFirstName = people.Objects.All(p => p.FirstName == firstPerson.FirstName)
            ? firstPerson.FirstName
            : null;

        SharedSurname = people.Objects.All(p => p.Surname == firstPerson.Surname)
            ? firstPerson.Surname
            : null;

        SharedNickname = people.Objects.All(p => p.Nickname == firstPerson.Nickname)
            ? firstPerson.Nickname
            : null;

        SharedAddress = people.Objects.All(p => p.Address == firstPerson.Address)
            ? firstPerson.Address
            : null;

        SharedZipCode = people.Objects.All(p => p.ZipCode == firstPerson.ZipCode)
            ? firstPerson.ZipCode
            : null;

        SharedCity = people.Objects.All(p => p.City == firstPerson.City)
            ? firstPerson.City
            : null;

        SharedBirthday = people.Objects.All(p => p.Birthday == firstPerson.Birthday)
            ? firstPerson.Birthday
            : null;

        SharedCategory = people.Objects.All(p => p.Category == firstPerson.Category)
            ? firstPerson.Category
            : null;

        SharedLatitude = people.Objects.All(p => p.Latitude == firstPerson.Latitude)
            ? firstPerson.Latitude
            : null;

        SharedLongitude = people.Objects.All(p => p.Longitude == firstPerson.Longitude)
            ? firstPerson.Longitude
            : null;

        OriginalSharedValues = new Person
        {
            FirstName = SharedFirstName,
            Surname = SharedSurname,
            Nickname = SharedNickname,
            Address = SharedAddress,
            ZipCode = SharedZipCode,
            City = SharedCity,
            Birthday = SharedBirthday,
            Category = SharedCategory,
            Latitude = SharedLatitude,
            Longitude = SharedLongitude
        };

        ApplyChangesCommand.RaiseCanExecuteChanged();
    }

    private async Task ApplyChanges()
    {

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
            Nickname = SharedNickname != OriginalSharedValues.Nickname ? SharedNickname : p.Nickname,
            Address = SharedAddress != OriginalSharedValues.Address ? SharedAddress : p.Address,
            ZipCode = SharedZipCode != OriginalSharedValues.ZipCode ? SharedZipCode : p.ZipCode,
            City = SharedCity != OriginalSharedValues.City ? SharedCity : p.City,
            Birthday = SharedBirthday != OriginalSharedValues.Birthday ? SharedBirthday : p.Birthday,
            Category = SharedCategory != OriginalSharedValues.Category ? SharedCategory : p.Category,
            Latitude = SharedLatitude != OriginalSharedValues.Latitude ? SharedLatitude : p.Latitude,
            Longitude = SharedLongitude != OriginalSharedValues.Longitude ? SharedLongitude : p.Longitude
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
            SharedSurname != OriginalSharedValues.Surname ||
            SharedNickname != OriginalSharedValues.Nickname ||
            SharedAddress != OriginalSharedValues.Address ||
            SharedZipCode != OriginalSharedValues.ZipCode ||
            SharedCity != OriginalSharedValues.City ||
            SharedBirthday != OriginalSharedValues.Birthday ||
            SharedCategory != OriginalSharedValues.Category ||
            SharedLatitude != OriginalSharedValues.Latitude ||
            SharedLongitude != OriginalSharedValues.Longitude;
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
        RaisePropertyChanged("SharedFirstName");
        RaisePropertyChanged("SharedSurname");
        RaisePropertyChanged("SharedNickname");
        RaisePropertyChanged("SharedAddress");
        RaisePropertyChanged("SharedZipCode");
        RaisePropertyChanged("SharedCity");
        RaisePropertyChanged("SharedBirthday");
        RaisePropertyChanged("SharedCategory");
        RaisePropertyChanged("SharedLatitude");
        RaisePropertyChanged("SharedLongitude");
    }

    private void Validate()
    {
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