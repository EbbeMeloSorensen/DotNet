using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class PersonListViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private IList<Record> _people;
        private Sorting _sorting;

        public FindPeopleViewModel FindPeopleViewModel { get; private set; }
        private ObservableCollection<PersonViewModel> _peopleViewModels;

        private RelayCommand<object> _selectionChangedCommand;
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

        public ObjectCollection<Record> SelectedPeople { get; private set; }

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

        public RelayCommand<object> SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<object>(SelectionChanged)); }
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

            _people = new List<Record>();

            SelectedPeople = new ObjectCollection<Record>();

            dataProvider.RecordCreated += (s, e) =>
            {
                if (!FindPeopleViewModel.PersonPassesFilter(e.Record))
                {
                    return;
                }

                _people.Add(e.Record);
                UpdatePersonViewModels();
            };

            dataProvider.RecordsUpdated += (s, e) =>
            {
                // Der vil nok i praksis som regel gælde, at de opdaterede personer, FØR opdateringen matchede
                // filteret, dvs at der potentielt vil skulle FJERNES personer fra viewet, men ikke TILFØJES
                // Som sådan burde det være muligt at undgå at skulle besøge repositoryet for at opdatere viewet

                // .. men vi gør det lige alligevel indtil videre
                // spørgsmålet er sgu egentlig også, om man overhovedet bør fjerne personer fra filteret - det er jo ikke sikkert at det virker super intuitivt
                //RetrievePeopleMatchingFilterFromRepository();
                UpdatePersonViewModels();
            };

            dataProvider.RecordsDeleted += (s, e) =>
            {
                var countBefore = _people.Count;
                _people = _people.Except(e.People).ToList();
                var countAfter = _people.Count;

                if (countAfter < countBefore)
                {
                    UpdatePersonViewModels();
                }
            };
        }

        private void RetrievePeopleMatchingFilterFromRepository()
        {
            // Det her kan den ikke - det munder ud i følgende fejlbesked: "LINQ to Entities does not recognize the method ''
            // and this method cannot be translated into a store expression"
            //_people = _dataProvider.FindPeople(p => FindPeopleViewModel.PersonPassesFilter(p));

            _people = _dataProvider.FindRecords(FindPeopleViewModel.FilterAsExpression());
        }

        private int CountPeopleMatchingFilterFromRepository()
        {
            return _dataProvider.CountRecords(FindPeopleViewModel.FilterAsExpression());
        }

        private void UpdateSorting()
        {
            switch (Sorting)
            {
                case Sorting.Name:
                    _people = _people.OrderBy(p => p.Term).ToList();
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
            // Selected people should stay selected - prøv lige at drop det indtil videre - hvornår er det overhovedet smart
            // Du antog vist, at det var smart efter følgende hændelser:
            // - Ændring af sortering
            // - Oprettelse af ny person
            // - Efter at have lavet bulk opdatering af en gruppe personer
            // - Efter at have slettet en gruppe af personer
            // - Efter at have fremsøgt personer

            //var idsOfSelectedPersons = new int[] { };

            //if (PersonViewModels != null)
            //{
            //    idsOfSelectedPersons = PersonViewModels
            //        .Where(pvm => pvm.IsSelected)
            //        .Select(pvm => pvm.Person.Id)
            //        .ToArray();
            //}

            UpdateSorting();

            PersonViewModels = new ObservableCollection<PersonViewModel>(_people.Select(p => new PersonViewModel
            {
                //IsSelected = idsOfSelectedPersons.Contains(p.Id),
                Person = p
            }));
        }

        private void SelectionChanged(object commandParameter)
        {
            IList temp = (IList)commandParameter;
            var peopleViewModels = temp.Cast<PersonViewModel>();

            UpdatePeopleSelection(peopleViewModels.Select(pvm => pvm.Person));
        }

        private void UpdatePeopleSelection(IEnumerable<Record> people)
        {
            SelectedPeople.Objects = people;
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
