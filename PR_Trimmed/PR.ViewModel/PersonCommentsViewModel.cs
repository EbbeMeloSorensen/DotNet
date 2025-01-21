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
        private ObservableCollection<PersonCommentListViewItemViewModel> _personCommentListViewItemViewModels;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public ObservableCollection<PersonCommentListViewItemViewModel> PersonCommentListViewItemViewModels
        {
            get => _personCommentListViewItemViewModels;
            set
            {
                _personCommentListViewItemViewModels = value;
                RaisePropertyChanged();
            }
        }

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
            _people.PropertyChanged += async (s, e) => await Initialize(s, e);
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
                PersonCommentListViewItemViewModels = new ObservableCollection<PersonCommentListViewItemViewModel>(
                    person.Comments.Select(pc => new PersonCommentListViewItemViewModel {PersonComment = pc}));
            }

            IsVisible = true;
        }
    }
}
