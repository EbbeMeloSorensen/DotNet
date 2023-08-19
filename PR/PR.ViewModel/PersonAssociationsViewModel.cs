using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PR.Application;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.ViewModel
{
    public class PersonAssociationsViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;

        private bool _isVisible;
        private ObjectCollection<Person> _people;
        private Person _activePerson;
        private ObservableCollection<PersonAssociationViewModel> _personAssociationViewModels;
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

        public ObjectCollection<PersonAssociation> SelectedPersonAssociations { get; private set; }

        public ObservableCollection<PersonAssociationViewModel> PersonAssociationViewModels
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

        public PersonAssociationsViewModel(
            IUIDataProvider dataProvider,
            IUnitOfWorkFactory unitOfWorkFactory,
            IDialogService applicationDialogService,
            ObjectCollection<Person> people)
        {
            _dataProvider = dataProvider;
            _unitOfWorkFactory = unitOfWorkFactory;
            _applicationDialogService = applicationDialogService;
            _people = people;
            SelectedPersonAssociations = new ObjectCollection<PersonAssociation>();

            // Den her lader til at være tung..
            _people.PropertyChanged += Initialize;
        }

        private void Initialize(object sender, PropertyChangedEventArgs e)
        {
            var temp = sender as ObjectCollection<Person>;

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
                PersonAssociationViewModels = new ObservableCollection<PersonAssociationViewModel>();
                return;
            }

            var person = _dataProvider.GetPersonWithAssociations(_activePerson.Id);

            PersonAssociationViewModels = new ObservableCollection<PersonAssociationViewModel>(person.ObjectPeople
                .Select(pa => new PersonAssociationViewModel
                {
                    PersonAssociation = pa
                })
                .Concat(person.SubjectPeople
                    .Select(pa => new PersonAssociationViewModel
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
            _dataProvider.DeletePersonAssociations(SelectedPersonAssociations.Objects.ToList());

            Populate();
        }

        private bool CanDeleteSelectedPersonAssociations()
        {
            return SelectedPersonAssociations.Objects != null &&
                   SelectedPersonAssociations.Objects.Any();
        }

        private void CreatePersonAssociation(object owner)
        {
            var dialogViewModel = new DefinePersonAssociationDialogViewModel(
                _dataProvider,
                _unitOfWorkFactory,
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
                _dataProvider.CreatePersonAssociation(new PersonAssociation
                {
                    SubjectPersonId = dialogViewModel.SubjectPerson.Id,
                    ObjectPersonId = dialogViewModel.ObjectPerson.Id,
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

            var dialogViewModel = new DefinePersonAssociationDialogViewModel(
                _dataProvider,
                _unitOfWorkFactory,
                _applicationDialogService,
                personAssociation.SubjectPerson,
                personAssociation.ObjectPerson,
                personAssociation.Description);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            personAssociation.Description = dialogViewModel.Description;
            personAssociation.SubjectPersonId = dialogViewModel.SubjectPerson.Id;
            personAssociation.ObjectPersonId = dialogViewModel.ObjectPerson.Id;

            _dataProvider.UpdatePersonAssociation(personAssociation);

            Populate();
        }

        private bool CanUpdatePersonAssociation(object owner)
        {
            return SelectedPersonAssociations.Objects != null &&
                   SelectedPersonAssociations.Objects.Count() == 1;
        }
    }
}
