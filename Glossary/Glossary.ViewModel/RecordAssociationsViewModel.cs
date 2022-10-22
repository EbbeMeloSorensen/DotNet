using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class RecordAssociationsViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;

        private bool _isVisible;
        private ObjectCollection<Record> _people;
        private Record _activePerson;
        private ObservableCollection<RecordAssociationViewModel> _personAssociationViewModels;
        private RelayCommand _selectionChangedCommand;
        private RelayCommand _deleteSelectedPersonAssociationsCommand;
        private RelayCommand<object> _createPersonAssociationCommand;
        private RelayCommand<object> _updatePersonAssociationCommand;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public ObjectCollection<RecordAssociation> SelectedPersonAssociations { get; private set; }

        public ObservableCollection<RecordAssociationViewModel> PersonAssociationViewModels
        {
            get { return _personAssociationViewModels; }
            set
            {
                _personAssociationViewModels = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand(SelectionChanged)); }
        }

        public RelayCommand DeleteSelectedPersonAssociationsCommand
        {
            get
            {
                return _deleteSelectedPersonAssociationsCommand ?? (
                           _deleteSelectedPersonAssociationsCommand = new RelayCommand(DeleteSelectedPersonAssociations, CanDeleteSelectedPersonAssociations));
            }
        }

        public RelayCommand<object> CreatePersonAssociationCommand
        {
            get
            {
                return _createPersonAssociationCommand ?? (
                           _createPersonAssociationCommand = new RelayCommand<object>(CreatePersonAssociation, CanCreatePersonAssociation));
            }
        }

        public RelayCommand<object> UpdatePersonAssociationCommand
        {
            get
            {
                return _updatePersonAssociationCommand ?? (
                           _updatePersonAssociationCommand = new RelayCommand<object>(UpdatePersonAssociation, CanUpdatePersonAssociation));
            }
        }

        public RecordAssociationsViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService,
            ObjectCollection<Record> people)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;
            _people = people;
            SelectedPersonAssociations = new ObjectCollection<RecordAssociation>();

            // Den her lader til at være tung..
            _people.PropertyChanged += Initialize;
        }

        private void Initialize(object sender, PropertyChangedEventArgs e)
        {
            var temp = sender as ObjectCollection<Record>;

            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                _activePerson = temp.Objects.Single();
                Populate();
                IsVisible = true;
            }
            else
            {
                _activePerson = null;
                IsVisible = false;
            }
        }

        private void Populate()
        {
            if (_activePerson == null)
            {
                PersonAssociationViewModels = new ObservableCollection<RecordAssociationViewModel>();
                return;
            }

            var person = _dataProvider.GetRecordWithAssociations(_activePerson.Id);

            PersonAssociationViewModels = new ObservableCollection<RecordAssociationViewModel>(person.ObjectRecords
                .Select(pa => new RecordAssociationViewModel
                {
                    PersonAssociation = pa
                })
                .Concat(person.SubjectRecords
                    .Select(pa => new RecordAssociationViewModel
                    {
                        PersonAssociation = pa
                    })));
        }

        private void SelectionChanged()
        {
            SelectedPersonAssociations.Objects = _personAssociationViewModels
                .Where(p => p.IsSelected)
                .Select(pa => pa.PersonAssociation);

            DeleteSelectedPersonAssociationsCommand.RaiseCanExecuteChanged();
            UpdatePersonAssociationCommand.RaiseCanExecuteChanged();
        }

        public void DeleteSelectedPersonAssociations()
        {
            _dataProvider.DeleteRecordAssociations(SelectedPersonAssociations.Objects.ToList());

            Populate();
        }

        private bool CanDeleteSelectedPersonAssociations()
        {
            return SelectedPersonAssociations.Objects != null &&
                   SelectedPersonAssociations.Objects.Any();
        }

        private void CreatePersonAssociation(object owner)
        {
            var dialogViewModel = new DefineRecordAssociationDialogViewModel(
                _dataProvider,
                _applicationDialogService,
                _activePerson,
                null,
                null);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            if (_activePerson != null)
            {
                _dataProvider.CreateRecordAssociation(new RecordAssociation
                {
                    SubjectRecordId = dialogViewModel.SubjectRecord.Id,
                    ObjectRecordId = dialogViewModel.ObjectRecord.Id,
                    Description = dialogViewModel.Description,
                    Created = DateTime.UtcNow
                });

                Populate();
            }
        }

        private bool CanCreatePersonAssociation(object owner)
        {
            return true;
        }

        private void UpdatePersonAssociation(object owner)
        {
            var personAssociation = SelectedPersonAssociations.Objects.Single();

            var dialogViewModel = new DefineRecordAssociationDialogViewModel(
                _dataProvider,
                _applicationDialogService,
                personAssociation.SubjectRecord,
                personAssociation.ObjectRecord,
                personAssociation.Description);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            personAssociation.Description = dialogViewModel.Description;
            personAssociation.SubjectRecordId = dialogViewModel.SubjectRecord.Id;
            personAssociation.ObjectRecordId = dialogViewModel.ObjectRecord.Id;

            _dataProvider.UpdateRecordAssociation(personAssociation);

            Populate();
        }

        private bool CanUpdatePersonAssociation(object owner)
        {
            return SelectedPersonAssociations.Objects != null &&
                   SelectedPersonAssociations.Objects.Count() == 1;
        }
    }
}
