using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Craft.Utils;
using PR.Domain.Entities.PR;
using PR.Persistence;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PR.ViewModel
{
    public class PersonCommentsViewModel : ViewModelBase
    {
        private bool _isVisible;
        private ObjectCollection<Person> _people;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public ObservableCollection<PersonCommentListViewItemViewModel> PersonCommentListViewItemViewModels { get; }

        public ObservableCollection<PersonCommentListViewItemViewModel> SelectedPersonCommentListViewItemViewModels { get; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
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

            _people.PropertyChanged += async (s, e) => await Initialize(s, e);

            SelectedPersonCommentListViewItemViewModels.CollectionChanged += (s, e) =>
            {
                var a = 0;
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
    }
}
