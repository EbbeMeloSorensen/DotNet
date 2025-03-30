using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModel.Utils;
using PR.Application;
using PR.Persistence;
using PR.Domain.Entities.PR;

namespace PR.ViewModel;

public class PeoplePropertiesViewModel : ViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private ObjectCollection<Person> _people;

    public Person OriginalSharedValues { get; private set; }
    public Person SharedValues { get; private set; }

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
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string? SharedSurname
    {
        get => SharedValues.Surname;
        set
        {
            SharedValues.Surname = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string? SharedNickname
    {
        get => SharedValues.Nickname;
        set
        {
            SharedValues.Nickname = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string? SharedAddress
    {
        get => SharedValues.Address;
        set
        {
            SharedValues.Address = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string? SharedZipCode
    {
        get => SharedValues.ZipCode;
        set
        {
            SharedValues.ZipCode = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string? SharedCity
    {
        get => SharedValues.City;
        set
        {
            SharedValues.City = value;
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
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string? SharedCategory
    {
        get => SharedValues.Category;
        set
        {
            SharedValues.Category = value;
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
        _state = StateOfView.Initial;
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
        UpdateState(StateOfView.Updated);

        Error = string.Join("",
            ValidationMessages.Select(e => e.ErrorMessage).ToArray());

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

    public ObservableCollection<ValidationError> ValidationMessages
    {
        get
        {
            if (_validationMessages == null)
            {
                _validationMessages = new ObservableCollection<ValidationError>
                {
                    new ValidationError {PropertyName = "SharedFirstName"},
                    new ValidationError {PropertyName = "SharedSurname"},
                    new ValidationError {PropertyName = "SharedNickname"},
                    new ValidationError {PropertyName = "SharedAddress"},
                    new ValidationError {PropertyName = "SharedZipCode"},
                    new ValidationError {PropertyName = "SharedCity"},
                    new ValidationError {PropertyName = "SharedBirthday"},
                    new ValidationError {PropertyName = "SharedCategory"},
                    new ValidationError {PropertyName = "SharedLatitude"},
                    new ValidationError {PropertyName = "SharedLongitude"},
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
                    case "SharedFirstName":
                    {
                        if (string.IsNullOrEmpty(SharedFirstName))
                        {
                            if (_people.Objects.Count() == 1)
                            {
                                errorMessage = "First name is required";
                            }
                        }
                        else if (SharedFirstName.Length > 127)
                        {
                            errorMessage = "First name cannot exceed 127 characters";
                        }

                        break;
                    }
                    case "SharedSurname":
                    {
                        if (SharedSurname != null && SharedSurname.Length > 255)
                        {
                            errorMessage = "Surname cannot exceed 255 characters";
                        }

                        break;
                    }
                    case "SharedNickname":
                    {
                        if (SharedNickname != null && SharedNickname.Length > 127)
                        {
                            errorMessage = "Nickname cannot exceed 127 characters";
                        }

                        break;
                    }
                    case "SharedAddress":
                    {
                        if (SharedAddress != null && SharedAddress.Length > 511)
                        {
                            errorMessage = "Address cannot exceed 511 characters";
                        }

                        break;
                    }
                    case "SharedZipCode":
                    {
                        if (SharedZipCode != null && SharedZipCode.Length > 127)
                        {
                            errorMessage = "Zip code cannot exceed 127 characters";
                        }

                        break;
                    }
                    case "SharedCity":
                    {
                        if (SharedCity != null && SharedCity.Length > 255)
                        {
                            errorMessage = "City cannot exceed 255 characters";
                        }

                        break;
                    }
                    case "SharedCategory":
                    {
                        if (SharedCategory != null && SharedCategory.Length > 127)
                        {
                            errorMessage = "Category cannot exceed 127 characters";
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

    private void UpdateState(StateOfView state)
    {
        _state = state;
        RaisePropertyChanges();
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