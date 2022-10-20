using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Craft.UI.Utils;
using Craft.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel;

public class PeoplePropertiesViewModel : ViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private readonly IUIDataProvider _dataProvider;
    private ObjectCollection<Person> _people;

    private string _originalSharedTerm;
    private string _originalSharedAddress;
    private string _originalSharedCategory;
    private string _originalSharedComments;

    private string _sharedTerm;
    private string _sharedAddress;
    private string _sharedCategory;
    private string _sharedComments;

    private bool _isVisible;

    private RelayCommand _applyChangesCommand;

    public string SharedTerm
    {
        get { return _sharedTerm; }
        set
        {
            _sharedTerm = value;
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
        IUIDataProvider dataProvider,
        ObjectCollection<Person> people)
    {
        _dataProvider = dataProvider;
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

        SharedTerm = temp.Objects.All(p => p.Term == firstPerson.Term)
            ? firstPerson.Term
            : null;

        SharedAddress = temp.Objects.All(p => p.Address == firstPerson.Address)
            ? firstPerson.Address
            : null;

        SharedCategory = temp.Objects.All(p => p.Category == firstPerson.Category)
            ? firstPerson.Category
            : null;

        SharedComments = temp.Objects.All(p => p.Description == firstPerson.Description)
            ? firstPerson.Description
            : null;

        _originalSharedTerm = SharedTerm;
        _originalSharedAddress = SharedAddress;
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
            Term = SharedTerm != _originalSharedTerm ? SharedTerm : p.Term,
            Address = SharedAddress != _originalSharedAddress ? SharedAddress : p.Address,
            Category = SharedCategory != _originalSharedCategory ? SharedCategory : p.Category,
            Description = SharedComments != _originalSharedComments ? SharedComments : p.Description,
            Created = p.Created
        }).ToList();

        _dataProvider.UpdatePeople(updatedPeople);
    }

    private bool CanApplyChanges()
    {
        return
            SharedTerm != _originalSharedTerm ||
            SharedAddress != _originalSharedAddress ||
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
                    new ValidationError {PropertyName = "SharedTerm"},
                    new ValidationError {PropertyName = "SharedAddress"},
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
                    case "SharedTerm":
                    {
                        if (string.IsNullOrEmpty(SharedTerm))
                        {
                            if (_people.Objects.Count() == 1)
                            {
                                errorMessage = "Term is required";
                            }
                        }
                        else if (SharedTerm.Length > 127)
                        {
                            errorMessage = "Term cannot exceed 127 characters";
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
        RaisePropertyChanged("SharedTerm");
        RaisePropertyChanged("SharedAddress");
        RaisePropertyChanged("SharedCategory");
        RaisePropertyChanged("SharedComments");
    }

    private void UpdateState(StateOfView state)
    {
        _state = state;
        RaisePropertyChanges();
    }
}