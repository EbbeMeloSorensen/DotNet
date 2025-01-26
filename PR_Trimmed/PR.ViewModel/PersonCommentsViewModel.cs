using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModel.Utils;
using PR.Domain.Entities.PR;
using PR.Persistence;

namespace PR.ViewModel
{
    public class PersonCommentsViewModel : ViewModelBase
    {
        private bool _isVisible;
        private ObjectCollection<Person> _people;

        private AsyncCommand<object> _createPersonCommentCommand;
        private AsyncCommand<object> _updatePersonCommentCommand;
        private AsyncCommand<object> _deletePersonCommentsCommand;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public ObservableCollection<PersonCommentListViewItemViewModel> PersonCommentListViewItemViewModels { get; }

        public ObservableCollection<PersonCommentListViewItemViewModel> SelectedPersonCommentListViewItemViewModels { get; }

        public ObjectCollection<PersonComment> SelectedPersonComments { get; }

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

        public PersonCommentsViewModel(
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

            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            person = await unitOfWork.People.GetIncludingComments(person.ID);

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
    }
}
