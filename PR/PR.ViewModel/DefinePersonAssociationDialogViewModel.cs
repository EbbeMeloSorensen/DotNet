using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using PR.Application;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.ViewModel;

public class DefinePersonAssociationDialogViewModel : DialogViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private Person _originalSubjectPerson;
    private Person _originalObjectPerson;
    private string _originalDescription;
    private Person _subjectPerson;
    private Person _objectPerson;
    private string _description;

    public PersonListViewModel PersonListViewModelSubject { get; private set; }
    public PersonListViewModel PersonListViewModelObject { get; private set; }

    private RelayCommand<object> _okCommand;
    private RelayCommand<object> _cancelCommand;

    public string Description
    {
        get { return _description; }
        set
        {
            _description = value;
            RaisePropertyChanged();
            OKCommand.RaiseCanExecuteChanged();
        }
    }

    public Person SubjectPerson
    {
        get { return _subjectPerson; }
        set
        {
            _subjectPerson = value;
            RaisePropertyChanged();
            OKCommand.RaiseCanExecuteChanged();
        }
    }

    public Person ObjectPerson
    {
        get { return _objectPerson; }
        set
        {
            _objectPerson = value;
            RaisePropertyChanged();
            OKCommand.RaiseCanExecuteChanged();
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

    public DefinePersonAssociationDialogViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService,
        Person subjectPerson,
        Person objectPerson,
        string description)
    {
        _originalSubjectPerson = SubjectPerson = subjectPerson;
        _originalObjectPerson = ObjectPerson = objectPerson;
        _originalDescription = Description = description;

        PersonListViewModelSubject = new PersonListViewModel(unitOfWorkFactory, applicationDialogService);
        PersonListViewModelObject = new PersonListViewModel(unitOfWorkFactory, applicationDialogService);

        PersonListViewModelSubject.SelectedPeople.PropertyChanged += (s, e) =>
        {
            var temp = s as ObjectCollection<Person>;
            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                SubjectPerson = temp.Objects.Single();
            }

            OKCommand.RaiseCanExecuteChanged();
        };

        PersonListViewModelObject.SelectedPeople.PropertyChanged += (s, e) =>
        {
            var temp = s as ObjectCollection<Person>;
            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                ObjectPerson = temp.Objects.Single();
            }

            OKCommand.RaiseCanExecuteChanged();
        };
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
        return
            SubjectPerson != null &&
            ObjectPerson != null &&
            Description != null &&
            !string.IsNullOrEmpty(Description) &&
            (SubjectPerson != _originalSubjectPerson ||
             ObjectPerson != _originalObjectPerson ||
             Description != _originalDescription);
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
                    case "Description":
                    {
                        if (Description.Length > 10 /*255*/)
                        {
                            errorMessage = "Description cannot exceed 255 characters";
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
                    new ValidationError {PropertyName = "Description"},
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
        RaisePropertyChanged("Description");
    }

    private void UpdateState(StateOfView state)
    {
        _state = state;
        RaisePropertyChanges();
    }
}