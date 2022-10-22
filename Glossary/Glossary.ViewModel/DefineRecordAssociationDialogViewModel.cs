using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel;

public class DefineRecordAssociationDialogViewModel : DialogViewModelBase, IDataErrorInfo
{
    private StateOfView _state;
    private ObservableCollection<ValidationError> _validationMessages;
    private string _error = string.Empty;

    private Record _originalSubjectRecord;
    private Record _originalObjectRecord;
    private string _originalDescription;
    private Record _subjectRecord;
    private Record _objectRecord;
    private string _description;

    public RecordListViewModel RecordListViewModelSubject { get; private set; }
    public RecordListViewModel RecordListViewModelObject { get; private set; }

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

    public Record SubjectRecord
    {
        get { return _subjectRecord; }
        set
        {
            _subjectRecord = value;
            RaisePropertyChanged();
            OKCommand.RaiseCanExecuteChanged();
        }
    }

    public Record ObjectRecord
    {
        get { return _objectRecord; }
        set
        {
            _objectRecord = value;
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

    public DefineRecordAssociationDialogViewModel(
        IUIDataProvider dataProvider,
        IDialogService applicationDialogService,
        Record subjectRecord,
        Record objectRecord,
        string description)
    {
        _originalSubjectRecord = SubjectRecord = subjectRecord;
        _originalObjectRecord = ObjectRecord = objectRecord;
        _originalDescription = Description = description;

        RecordListViewModelSubject = new RecordListViewModel(dataProvider, applicationDialogService);
        RecordListViewModelObject = new RecordListViewModel(dataProvider, applicationDialogService);

        RecordListViewModelSubject.SelectedPeople.PropertyChanged += (s, e) =>
        {
            var temp = s as ObjectCollection<Record>;
            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                SubjectRecord = temp.Objects.Single();
            }

            OKCommand.RaiseCanExecuteChanged();
        };

        RecordListViewModelObject.SelectedPeople.PropertyChanged += (s, e) =>
        {
            var temp = s as ObjectCollection<Record>;
            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                ObjectRecord = temp.Objects.Single();
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
            SubjectRecord != null &&
            ObjectRecord != null &&
            Description != null &&
            !string.IsNullOrEmpty(Description) &&
            (SubjectRecord != _originalSubjectRecord ||
             ObjectRecord != _originalObjectRecord ||
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