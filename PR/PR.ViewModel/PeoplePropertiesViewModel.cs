using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Craft.UI.Utils;
using Craft.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PR.Application;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.ViewModel;

public class PeoplePropertiesViewModel : ViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private readonly UnitOfWorkFactoryFacade _unitOfWorkFactoryFacade;
    private ObjectCollection<Person> _people;

    private string _originalSharedFirstName;
    private string _originalSharedSurname;
    private string _originalSharedNickname;
    private string _originalSharedAddress;
    private string _originalSharedZipCode;
    private string _originalSharedCity;
    private DateTime? _originalSharedBirthday;
    private string _originalSharedCategory;
    private string _originalSharedComments;

    private string _sharedFirstName;
    private string _sharedSurname;
    private string _sharedNickname;
    private string _sharedAddress;
    private string _sharedZipCode;
    private string _sharedCity;
    private DateTime? _sharedBirthday;
    private string _sharedCategory;
    private string _sharedComments;

    private bool _isVisible;

    private RelayCommand _applyChangesCommand;

    public event EventHandler<PeopleEventArgs> PeopleUpdated;

    public string SharedFirstName
    {
        get { return _sharedFirstName; }
        set
        {
            _sharedFirstName = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedSurname
    {
        get { return _sharedSurname; }
        set
        {
            _sharedSurname = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedNickname
    {
        get { return _sharedNickname; }
        set
        {
            _sharedNickname = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedAddress
    {
        get { return _sharedAddress; }
        set
        {
            _sharedAddress = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedZipCode
    {
        get { return _sharedZipCode; }
        set
        {
            _sharedZipCode = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedCity
    {
        get { return _sharedCity; }
        set
        {
            _sharedCity = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public DateTime? SharedBirthday
    {
        get { return _sharedBirthday; }
        set
        {
            _sharedBirthday = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedCategory
    {
        get { return _sharedCategory; }
        set
        {
            _sharedCategory = value;
            RaisePropertyChanged();
            ApplyChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public string SharedComments
    {
        get { return _sharedComments; }
        set
        {
            _sharedComments = value;
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

    public PeoplePropertiesViewModel(
        UnitOfWorkFactoryFacade unitOfWorkFactoryFacade,
        ObjectCollection<Person> people)
    {
        _unitOfWorkFactoryFacade = unitOfWorkFactoryFacade;
        _people = people;
        _people.PropertyChanged += Initialize;
    }

    private void Initialize(object sender, PropertyChangedEventArgs e)
    {
        _state = StateOfView.Initial;
        var temp = sender as ObjectCollection<Person>;

        var firstPerson = temp?.Objects.FirstOrDefault();

        if (firstPerson == null)
        {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        SharedFirstName = temp.Objects.All(p => p.FirstName == firstPerson.FirstName)
            ? firstPerson.FirstName
            : null;

        SharedSurname = temp.Objects.All(p => p.Surname == firstPerson.Surname)
            ? firstPerson.Surname
            : null;

        SharedNickname = temp.Objects.All(p => p.Nickname == firstPerson.Nickname)
            ? firstPerson.Nickname
            : null;

        SharedAddress = temp.Objects.All(p => p.Address == firstPerson.Address)
            ? firstPerson.Address
            : null;

        SharedZipCode = temp.Objects.All(p => p.ZipCode == firstPerson.ZipCode)
            ? firstPerson.ZipCode
            : null;

        SharedCity = temp.Objects.All(p => p.City == firstPerson.City)
            ? firstPerson.City
            : null;

        SharedBirthday = temp.Objects.All(p => p.Birthday == firstPerson.Birthday)
            ? firstPerson.Birthday
            : null;

        SharedCategory = temp.Objects.All(p => p.Category == firstPerson.Category)
            ? firstPerson.Category
            : null;

        SharedComments = temp.Objects.All(p => p.Description == firstPerson.Description)
            ? firstPerson.Description
            : null;

        _originalSharedFirstName = SharedFirstName;
        _originalSharedSurname = SharedSurname;
        _originalSharedNickname = SharedNickname;
        _originalSharedAddress = SharedAddress;
        _originalSharedZipCode = SharedZipCode;
        _originalSharedCity = SharedCity;
        _originalSharedBirthday = SharedBirthday;
        _originalSharedCategory = SharedCategory;
        _originalSharedComments = SharedComments;

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

        var updatedPeople = _people.Objects.Select(p => new Person
        {
            Id = p.Id,
            ObjectId = p.ObjectId,
            FirstName = SharedFirstName != _originalSharedFirstName ? SharedFirstName : p.FirstName,
            Surname = SharedSurname != _originalSharedSurname ? SharedSurname : p.Surname,
            Nickname = SharedNickname != _originalSharedNickname ? SharedNickname : p.Nickname,
            Address = SharedAddress != _originalSharedAddress ? SharedAddress : p.Address,
            ZipCode = SharedZipCode != _originalSharedZipCode ? SharedZipCode : p.ZipCode,
            City = SharedCity != _originalSharedCity ? SharedCity : p.City,
            Birthday = SharedBirthday != _originalSharedBirthday
                ? new DateTime(
                    SharedBirthday.Value.Year,
                    SharedBirthday.Value.Month,
                    SharedBirthday.Value.Day,
                    0, 0, 0, DateTimeKind.Utc) 
                : p.Birthday,
            Category = SharedCategory != _originalSharedCategory ? SharedCategory : p.Category,
            Description = SharedComments != _originalSharedComments ? SharedComments : p.Description,
            Created = p.Created
        }).ToList();

        using (var unitOfWork = _unitOfWorkFactoryFacade.GenerateUnitOfWork())
        {
            unitOfWork.People.UpdateRange(updatedPeople);
            unitOfWork.Complete();
        }

        OnPeopleUpdated(updatedPeople);
    }

    private bool CanApplyChanges()
    {
        return
            SharedFirstName != _originalSharedFirstName ||
            SharedSurname != _originalSharedSurname ||
            SharedNickname != _originalSharedNickname ||
            SharedAddress != _originalSharedAddress ||
            SharedZipCode != _originalSharedZipCode ||
            SharedCity != _originalSharedCity ||
            SharedBirthday != _originalSharedBirthday ||
            SharedCategory != _originalSharedCategory ||
            SharedComments != _originalSharedComments;
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
                    new ValidationError {PropertyName = "SharedComments"}
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
                    case "SharedComments":
                    {
                        if (SharedComments != null && SharedComments.Length > 2047)
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
        RaisePropertyChanged("SharedComments");
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