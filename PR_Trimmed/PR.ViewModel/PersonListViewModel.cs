using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Domain;
using PR.Persistence;
using PR.Domain.Entities.PR;

namespace PR.ViewModel
{
    public class PersonListViewModel : ViewModelBase
    {
        private readonly IDialogService _applicationDialogService;
        private IList<Person> _people;
        private Sorting _sorting;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public FindPeopleViewModel FindPeopleViewModel { get; }

        private AsyncCommand<object> _findPeopleCommand;

        public ObservableCollection<PersonViewModel> PersonViewModels { get; }

        public ObservableCollection<PersonViewModel> SelectedPersonViewModels { get; }

        public ObjectCollection<Person> SelectedPeople { get; }

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

        public AsyncCommand<object> FindPeopleCommand
        {
            get
            {
                return _findPeopleCommand ?? (_findPeopleCommand = new AsyncCommand<object>(FindPeople));
            }
        }

        public PersonListViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDialogService applicationDialogService)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            _applicationDialogService = applicationDialogService;
            _sorting = Sorting.Name;

            FindPeopleViewModel = new FindPeopleViewModel();

            _people = new List<Person>();

            PersonViewModels = new ObservableCollection<PersonViewModel>();
            SelectedPersonViewModels = new ObservableCollection<PersonViewModel>();
            SelectedPeople = new ObjectCollection<Person>();

            SelectedPersonViewModels.CollectionChanged += (s, e) =>
            {
                SelectedPeople.Objects = SelectedPersonViewModels.Select(_ => _.Person);
            };
        }

        public void AddPerson(
            Person person)
        {
            _people.Add(person);
            UpdatePersonViewModels();

            SelectedPersonViewModels.Clear();

            foreach (var personViewModel in PersonViewModels)
            {
                if (personViewModel.Person.ID != person.ID) continue;

                SelectedPersonViewModels.Add(personViewModel);
                break;
            }
        }

        public void UpdatePeople(
            IEnumerable<Person> people)
        {
            var idsOfUpdatedPeople = people.Select(_ => _.ID).ToList();

            foreach (var person in _people)
            {
                if (idsOfUpdatedPeople.Contains(person.ID))
                {
                    person.CopyAttributes(people.Single(_ => _.ID == person.ID));
                }
            }

            UpdatePersonViewModels();

            SelectedPersonViewModels.Clear();

            foreach (var personViewModel in PersonViewModels)
            {
                if (idsOfUpdatedPeople.Contains(personViewModel.Person.ID))
                {
                    SelectedPersonViewModels.Add(personViewModel);
                }
            }
        }

        public void RemovePeople(
            IEnumerable<Person> people)
        {
            var idsOfDeletedPeople = people.Select(_ => _.ID);

            _people = _people
                .Where(_ => !idsOfDeletedPeople.Contains(_.ID))
                .ToList();

            try
            {
                SelectedPersonViewModels.Clear();
                UpdatePersonViewModels();
            }
            catch (Exception e)
            {
                var message = e.Message;
            }
        }

        private async Task RetrievePeopleMatchingFilterFromRepository()
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                _people = (await unitOfWork.People.Find(FindPeopleViewModel.FilterAsExpression())).ToList();
            }
        }

        private int CountPeopleMatchingFilterFromRepository()
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                return unitOfWork.People.Count(FindPeopleViewModel.FilterAsExpression());
            }
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

            PersonViewModels.Clear();

            _people.ToList().ForEach(person =>
            {
                PersonViewModels.Add(new PersonViewModel { Person = person });
            });
        }

        private async Task FindPeople(object owner)
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

            await RetrievePeopleMatchingFilterFromRepository();
            UpdatePersonViewModels();
        }
    }
}
