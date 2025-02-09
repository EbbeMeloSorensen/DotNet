using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModel.Utils;
using PR.Domain.Entities.PR;
using PR.Persistence;
using GalaSoft.MvvmLight.Command;

namespace PR.ViewModel
{
    public class PersonPropertiesViewModel : ViewModelBase
    {
        private bool _isVisible;
        private ObjectCollection<Person> _people;

        private AsyncCommand<object> _createPersonCommentCommand;
        private AsyncCommand<object> _updatePersonCommentCommand;
        private AsyncCommand<object> _deletePersonCommentsCommand;

        private RelayCommand<object> _personVariantSelectionChangedCommand;

        private AsyncCommand<object> _createPersonVariantCommand;
        private AsyncCommand<object> _updatePersonVariantCommand;
        private AsyncCommand<object> _deletePersonVariantsCommand;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public ObservableCollection<PersonCommentListViewItemViewModel> PersonCommentListViewItemViewModels { get; }

        public ObservableCollection<PersonCommentListViewItemViewModel> SelectedPersonCommentListViewItemViewModels { get; }

        public ObjectCollection<PersonComment> SelectedPersonComments { get; }

        public ObservableCollection<PersonVariantListViewItemViewModel> PersonVariantListViewItemViewModels { get; }

        public ObservableCollection<PersonVariantListViewItemViewModel> SelectedPersonVariantListViewItemViewModels { get; }
        
        public ObjectCollection<Person> SelectedPersonVariants { get; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public AsyncCommand<object> CreatePersonCommentCommand
        {
            get
            {
                return _createPersonCommentCommand ??
                       (_createPersonCommentCommand = new AsyncCommand<object>(CreatePersonComment));
            }
        }

        public AsyncCommand<object> UpdatePersonCommentCommand
        {
            get
            {
                return _updatePersonCommentCommand ??
                       (_updatePersonCommentCommand = new AsyncCommand<object>(UpdatePersonComment, CanUpdatePersonComment));
            }
        }

        public AsyncCommand<object> DeletePersonCommentsCommand
        {
            get
            {
                return _deletePersonCommentsCommand ??
                       (_deletePersonCommentsCommand = new AsyncCommand<object>(DeletePersonComments, CanDeletePersonComments));
            }
        }

        public RelayCommand<object> PersonVariantSelectionChangedCommand
        {
            get { return _personVariantSelectionChangedCommand ?? (_personVariantSelectionChangedCommand = new RelayCommand<object>(PersonVariantSelectionChanged)); }
        }

        public AsyncCommand<object> CreatePersonVariantCommand
        {
            get
            {
                return _createPersonVariantCommand ??
                       (_createPersonVariantCommand = new AsyncCommand<object>(CreatePersonVariant));
            }
        }

        public AsyncCommand<object> UpdatePersonVariantCommand
        {
            get
            {
                return _updatePersonVariantCommand ??
                       (_updatePersonVariantCommand = new AsyncCommand<object>(UpdatePersonVariant, CanUpdatePersonVariant));
            }
        }

        public AsyncCommand<object> DeletePersonVariantsCommand
        {
            get
            {
                return _deletePersonVariantsCommand ??
                       (_deletePersonVariantsCommand = new AsyncCommand<object>(DeletePersonVariants, CanDeletePersonVariants));
            }
        }

        public PersonPropertiesViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            ObjectCollection<Person> people)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            _people = people;

            PersonCommentListViewItemViewModels =
                new ObservableCollection<PersonCommentListViewItemViewModel>();

            SelectedPersonCommentListViewItemViewModels =
                new ObservableCollection<PersonCommentListViewItemViewModel>();

            SelectedPersonComments = new ObjectCollection<PersonComment>();

            PersonVariantListViewItemViewModels = new ObservableCollection<PersonVariantListViewItemViewModel>();

            SelectedPersonVariantListViewItemViewModels =
                new ObservableCollection<PersonVariantListViewItemViewModel>();

            SelectedPersonVariants = new ObjectCollection<Person>();

            _people.PropertyChanged += async (s, e) => await Initialize(s, e);

            SelectedPersonCommentListViewItemViewModels.CollectionChanged += (s, e) =>
            {
                SelectedPersonComments.Objects = SelectedPersonCommentListViewItemViewModels.Select(_ => _.PersonComment);
            };

            SelectedPersonComments.PropertyChanged += (s, e) =>
            {
                UpdatePersonCommentCommand.RaiseCanExecuteChanged();
                DeletePersonCommentsCommand.RaiseCanExecuteChanged();
            };

            SelectedPersonVariantListViewItemViewModels.CollectionChanged += (s, e) =>
            {
                SelectedPersonVariants.Objects = SelectedPersonVariantListViewItemViewModels.Select(_ => _.PersonVariant);
            };
        }

        private async Task Initialize(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (_people.Objects.Count() != 1)
            {
                IsVisible = false;
                return;
            }

            var person = _people.Objects.Single();

            using var unitOfWork1 = UnitOfWorkFactory.GenerateUnitOfWork();
            person = await unitOfWork1.People.GetIncludingComments(person.ID);

            if (person.Comments != null)
            {
                PersonCommentListViewItemViewModels.Clear();

                person.Comments.ToList().ForEach(pc =>
                {
                    PersonCommentListViewItemViewModels.Add(new PersonCommentListViewItemViewModel
                    {
                        PersonComment = pc
                    });
                });
            }

            using var unitOfWork2 = UnitOfWorkFactory.GenerateUnitOfWork();
            var personVariants = await unitOfWork2.People.GetAllVariants(person.ID);

            PersonVariantListViewItemViewModels.Clear();
            personVariants.ToList().ForEach(pv =>
            {
                PersonVariantListViewItemViewModels.Add(new PersonVariantListViewItemViewModel
                {
                    PersonVariant = pv
                });
            });

            IsVisible = true;
        }

        private async Task CreatePersonComment(
            object owner)
        {
            throw new NotImplementedException();
        }

        private async Task UpdatePersonComment(
            object owner)
        {
            throw new NotImplementedException();
        }

        private bool CanUpdatePersonComment(
            object owner)
        {
            return SelectedPersonComments.Objects != null && SelectedPersonComments.Objects.Count() == 1;
        }

        private async Task DeletePersonComments(
            object owner)
        {
            throw new NotImplementedException();
        }

        private bool CanDeletePersonComments(
            object owner)
        {
            return SelectedPersonComments.Objects != null && SelectedPersonComments.Objects.Count() > 1;
        }

        private void PersonVariantSelectionChanged(
            object obj)
        {
            var temp = (IList)obj;

            var selectedPersonVariantListViewItemViewModels = temp.Cast<PersonVariantListViewItemViewModel>();

            SelectedPersonVariants.Objects = selectedPersonVariantListViewItemViewModels.Select(_ => _.PersonVariant);

            UpdatePersonVariantCommand.RaiseCanExecuteChanged();
            DeletePersonVariantsCommand.RaiseCanExecuteChanged();
        }

        private async Task CreatePersonVariant(
            object owner)
        {
            throw new NotImplementedException();
        }

        private async Task UpdatePersonVariant(
            object owner)
        {
            throw new NotImplementedException();
        }

        private bool CanUpdatePersonVariant(
            object owner)
        {
            return SelectedPersonVariants.Objects != null && SelectedPersonVariants.Objects.Count() == 1;
        }

        private async Task DeletePersonVariants(
            object owner)
        {
            throw new NotImplementedException();
        }

        private bool CanDeletePersonVariants(
            object owner)
        {
            return SelectedPersonVariants.Objects != null && SelectedPersonVariants.Objects.Any();
        }
    }
}
