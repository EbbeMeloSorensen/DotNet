using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Domain;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Domain.Entities.PR;
using PR.Persistence;

namespace PR.ViewModel
{
    public class PersonPropertiesViewModel : ViewModelBase
    {
        private readonly IDialogService _applicationDialogService;
        private IBusinessRuleCatalog _businessRuleCatalog;
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
                       (_deletePersonCommentsCommand = new AsyncCommand<object>(SoftDeleteSelectedPersonComments, CanSoftDeleteSelectedPersonComments));
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
            IDialogService applicationDialogService,
            IBusinessRuleCatalog businessRuleCatalog,
            ObjectCollection<Person> people)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            _applicationDialogService = applicationDialogService;
            _businessRuleCatalog = businessRuleCatalog;
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
                PersonVariantListViewItemViewModels.Add(new PersonVariantListViewItemViewModel{PersonVariant = pv});
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

        private async Task SoftDeleteSelectedPersonComments(
            object owner)
        {
            throw new NotImplementedException();
        }

        private bool CanSoftDeleteSelectedPersonComments(
            object owner)
        {
            return SelectedPersonComments.Objects != null &&
                   SelectedPersonComments.Objects.Any() &&
                   SelectedPersonComments.Objects.All(_ => _.End.Year == 9999);
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
            var a = 0;

            var occupiedDateRanges = PersonVariantListViewItemViewModels
                .Select(_ => _.PersonVariant)
                .Select(_ => new Tuple<DateTime, DateTime>(_.Start, _.End))
                .OrderBy(_ => _.Item1)
                .ToList();

            var dialogViewModel = new CreateOrUpdatePersonDialogViewModel(
                UnitOfWorkFactory,
                _businessRuleCatalog,
                null,
                occupiedDateRanges); 

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var personVariants = PersonVariantListViewItemViewModels
                .Select(_ => _.PersonVariant)
                .Append(dialogViewModel.Person)
                .OrderBy(_ => _.Start)
                .ToList();

            PersonVariantListViewItemViewModels.Clear();

            personVariants.ForEach(pv => PersonVariantListViewItemViewModels.Add(
                new PersonVariantListViewItemViewModel{PersonVariant = pv}));
        }

        private async Task UpdatePersonVariant(
            object owner)
        {
            var selectedPersonVariant = SelectedPersonVariants.Objects.Single();

            var otherPersonVariants = PersonVariantListViewItemViewModels
                .Select(_ => _.PersonVariant)
                .Where(_ => _ != selectedPersonVariant);

            var occupiedDateRanges = otherPersonVariants
                .Select(_ => new Tuple<DateTime, DateTime>(_.Start, _.End))
                .OrderBy(_ => _.Item1);

            var dialogViewModel = new CreateOrUpdatePersonDialogViewModel(
                UnitOfWorkFactory,
                _businessRuleCatalog,
                selectedPersonVariant,
                occupiedDateRanges);

            dialogViewModel.FirstName = selectedPersonVariant.FirstName;
            dialogViewModel.Surname = selectedPersonVariant.Surname;
            dialogViewModel.Nickname = selectedPersonVariant.Nickname;
            dialogViewModel.Address = selectedPersonVariant.Address;
            dialogViewModel.ZipCode = selectedPersonVariant.ZipCode;
            dialogViewModel.City = selectedPersonVariant.City;
            dialogViewModel.Birthday = selectedPersonVariant.Birthday;
            dialogViewModel.Category = selectedPersonVariant.Category;
            dialogViewModel.Latitude = selectedPersonVariant.Latitude == null ? "" : selectedPersonVariant.Latitude.Value.ToString(CultureInfo.InvariantCulture);
            dialogViewModel.Longitude = selectedPersonVariant.Longitude == null ? "" : selectedPersonVariant.Longitude.Value.ToString(CultureInfo.InvariantCulture);
            dialogViewModel.StartDate = selectedPersonVariant.Start;
            dialogViewModel.EndDate = selectedPersonVariant.End;

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.OK)
            {
                // Todo: Update the person properties view with the updated person variant.
                throw new NotImplementedException("UpdatePersonVariant is not implemented yet.");
            }
        }

        private bool CanUpdatePersonVariant(
            object owner)
        {
            return SelectedPersonVariants.Objects != null && SelectedPersonVariants.Objects.Count() == 1;
        }

        private async Task DeletePersonVariants(
            object owner)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                await unitOfWork.People.EraseRange(SelectedPersonVariants.Objects);
                unitOfWork.Complete();
            }

            var personVariants = PersonVariantListViewItemViewModels
                .Select(_ => _.PersonVariant)
                .Except(SelectedPersonVariants.Objects)
                .OrderBy(_ => _.Start)
                .ToList();

            PersonVariantListViewItemViewModels.Clear();

            personVariants.ForEach(pv => PersonVariantListViewItemViewModels.Add(
                new PersonVariantListViewItemViewModel{PersonVariant = pv}));
        }

        private bool CanDeletePersonVariants(
            object owner)
        {
            return SelectedPersonVariants.Objects != null && SelectedPersonVariants.Objects.Any();
        }
    }
}
