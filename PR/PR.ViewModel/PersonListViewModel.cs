using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using PR.Application;
using PR.Domain;
using PR.Domain.Entities;

namespace PR.ViewModel
{
    public class PersonListViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private IList<Person> _people;
        private Sorting _sorting;

        public FindPeopleViewModel FindPeopleViewModel { get; }
        private ObservableCollection<PersonViewModel> _peopleViewModels;

        private RelayCommand<object> _findPeopleCommand;

        public ObservableCollection<PersonViewModel> PersonViewModels
        {
            get { return _peopleViewModels; }
            set
            {
                _peopleViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObjectCollection<Person> SelectedPeople { get; }

        public ObservableCollection<PersonViewModel> SelectedPersonViewModels { get; set; }

        public Sorting Sorting
        {
            get { return _sorting; }
            set
            {
                _sorting = value;
                RaisePropertyChanged();
                UpdateSorting();
                UpdatePersonViewModels();
            }
        }

        public RelayCommand<object> FindPeopleCommand
        {
            get
            {
                return _findPeopleCommand ?? (_findPeopleCommand = new RelayCommand<object>(FindPeople));
            }
        }

        public PersonListViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;
            _sorting = Sorting.Name;

            FindPeopleViewModel = new FindPeopleViewModel();

            _people = new List<Person>();

            SelectedPersonViewModels = new ObservableCollection<PersonViewModel>();
            SelectedPeople = new ObjectCollection<Person>();

            dataProvider.PersonCreated += (s, e) =>
            {
                _people.Add(e.Person);
                UpdatePersonViewModels();

                SelectedPersonViewModels.Clear();

                foreach (var personViewModel in PersonViewModels)
                {
                    if (personViewModel.Person.Id != e.Person.Id) continue;

                    SelectedPersonViewModels.Add(personViewModel);
                    break;
                }
            };

            dataProvider.PeopleUpdated += (s, e) =>
            {
                var idsOfUpdatedPeople = e.People.Select(_ => _.Id).ToList();

                foreach (var person in _people)
                {
                    if (idsOfUpdatedPeople.Contains(person.Id))
                    {
                        person.CopyAttributes(e.People.Single(_ => _.Id == person.Id));
                    }
                }

                PersonViewModels = new ObservableCollection<PersonViewModel>(_people.Select(
                    p => new PersonViewModel { Person = p }));
                
                SelectedPersonViewModels.Clear();

                foreach (var personViewModel in PersonViewModels)
                {
                    if (idsOfUpdatedPeople.Contains(personViewModel.Person.Id))
                    {
                        SelectedPersonViewModels.Add(personViewModel);
                    }
                }
            };

            dataProvider.PeopleDeleted += (s, e) =>
            {
                var countBefore = _people.Count;
                _people = _people.Except(e.People).ToList();
                var countAfter = _people.Count;

                if (countAfter == countBefore) return;

                SelectedPersonViewModels.Clear();
                UpdatePersonViewModels();
            };

            SelectedPersonViewModels.CollectionChanged += (s, e) =>
            {
                SelectedPeople.Objects = SelectedPersonViewModels.Select(_ => _.Person);
            };
        }

        private void RetrievePeopleMatchingFilterFromRepository()
        {
            _people = _dataProvider.FindPeople(FindPeopleViewModel.FilterAsExpression());
        }

        private int CountPeopleMatchingFilterFromRepository()
        {
            return _dataProvider.CountPeople(FindPeopleViewModel.FilterAsExpression());
        }

        private void UpdateSorting()
        {
            switch (Sorting)
            {
                case Sorting.Name:
                    _people = _people.OrderBy(p => p.FirstName).ToList();
                    break;
                case Sorting.Created:
                    _people = _people.OrderByDescending(p => p.Created).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdatePersonViewModels()
        {
            UpdateSorting();

            PersonViewModels = new ObservableCollection<PersonViewModel>(_people.Select(
                p => new PersonViewModel { Person = p }));
        }

        private void FindPeople(object owner)
        {
            var personLimit = 10;
            var count = CountPeopleMatchingFilterFromRepository();

            if (count == 0)
            {
                var dialogViewModel = new MessageBoxDialogViewModel("No person matches the search criteria", false);
                _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
            }

            if (count > personLimit)
            {
                var dialogViewModel = new MessageBoxDialogViewModel($"{count} people match the search criteria.\nDo you want to retrieve them all from the repository?", true);
                if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
                {
                    return;
                }
            }

            RetrievePeopleMatchingFilterFromRepository();
            UpdatePersonViewModels();
        }
    }
}
