using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PR.Application;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;
        private readonly ILogger _logger;
        private string _mainWindowTitle;

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                _mainWindowTitle = value;
                RaisePropertyChanged();
            }
        }

        public PersonListViewModel PersonListViewModel { get; private set; }

        public PeoplePropertiesViewModel PeoplePropertiesViewModel { get; private set; }
        public PersonAssociationsViewModel PersonAssociationsViewModel { get; private set; }

        public LogViewModel LogViewModel { get; private set; }

        private RelayCommand<object> _createPersonCommand;
        private RelayCommand<object> _showOptionsDialogCommand;
        private RelayCommand _deleteSelectedPeopleCommand;
        private RelayCommand _exportPeopleCommand;
        private RelayCommand _exportSelectionToGraphmlCommand;
        private RelayCommand _importPeopleCommand;
        private RelayCommand _exitCommand;

        public RelayCommand DeleteSelectedPeopleCommand
        {
            get { return _deleteSelectedPeopleCommand ?? (_deleteSelectedPeopleCommand = new RelayCommand(DeleteSelectedPeople, CanDeleteSelectedPeople)); }
        }

        public RelayCommand<object> CreatePersonCommand
        {
            get { return _createPersonCommand ?? (_createPersonCommand = new RelayCommand<object>(CreatePerson, CanCreatePerson)); }
        }

        public RelayCommand<object> ShowOptionsDialogCommand
        {
            get { return _showOptionsDialogCommand ?? (_showOptionsDialogCommand = new RelayCommand<object>(ShowOptionsDialog, CanShowOptionsDialog)); }
        }

        public RelayCommand ExportPeopleCommand
        {
            get { return _exportPeopleCommand ?? (_exportPeopleCommand = new RelayCommand(ExportPeople, CanExportPeople)); }
        }

        public RelayCommand ExportSelectionToGraphmlCommand
        {
            get { return _exportSelectionToGraphmlCommand ?? (_exportSelectionToGraphmlCommand = new RelayCommand(
                ExportSelectionToGraphml, CanExportSelectionToGraphml)); }
        }

        public RelayCommand ImportPeopleCommand
        {
            get { return _importPeopleCommand ?? (_importPeopleCommand = new RelayCommand(ImportPeople, CanImportPeople)); }
        }

        public RelayCommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(Exit, CanExit)); }
        }

        public MainWindowViewModel(
            IUIDataProvider dataProvider,
            IUnitOfWorkFactory unitOfWorkFactory,
            IDialogService applicationDialogService,
            ILogger logger)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;

            LogViewModel = new LogViewModel();
            _logger = new ViewModelLogger(logger, LogViewModel);
            _dataProvider.Initialize(_logger);

            PersonListViewModel = new PersonListViewModel(dataProvider, unitOfWorkFactory, applicationDialogService);

            PersonListViewModel.SelectedPeople.PropertyChanged += HandlePeopleSelectionChanged;

            PeoplePropertiesViewModel = new PeoplePropertiesViewModel(
                dataProvider,
                PersonListViewModel.SelectedPeople);

            PersonAssociationsViewModel = new PersonAssociationsViewModel(
                dataProvider,
                unitOfWorkFactory,
                applicationDialogService,
                PersonListViewModel.SelectedPeople);

            _logger.WriteLine(LogMessageCategory.Information, "Application started");
        }

        private void HandlePeopleSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            DeleteSelectedPeopleCommand.RaiseCanExecuteChanged();
            ExportSelectionToGraphmlCommand.RaiseCanExecuteChanged();
        }

        private void CreatePerson(object owner)
        {
            var dialogViewModel = new CreatePersonDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var birthday = dialogViewModel.Birthday.HasValue
                ? new DateTime(
                    dialogViewModel.Birthday.Value.Year,
                    dialogViewModel.Birthday.Value.Month,
                    dialogViewModel.Birthday.Value.Day,
                    0, 0, 0, DateTimeKind.Utc)
                : new DateTime?();

            _dataProvider.CreatePerson(new Person
            {
                FirstName = dialogViewModel.FirstName,
                Surname = dialogViewModel.Surname,
                Nickname = dialogViewModel.Nickname,
                Address = dialogViewModel.Address,
                ZipCode = dialogViewModel.ZipCode,
                City = dialogViewModel.City,
                Birthday = birthday,
                Category = dialogViewModel.Category,
                Description = dialogViewModel.Comments,
                Created = DateTime.UtcNow
            });
        }

        private bool CanCreatePerson(object owner)
        {
            return true;
        }

        public void DeleteSelectedPeople()
        {
            _dataProvider.DeletePeople(PersonListViewModel.SelectedPeople.Objects.ToList());
        }

        private bool CanDeleteSelectedPeople()
        {
            return PersonListViewModel.SelectedPeople.Objects != null &&
                   PersonListViewModel.SelectedPeople.Objects.Any();
        }

        private void ExportPeople()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            _dataProvider.ExportData(dialog.FileName, null);
        }

        private bool CanExportPeople()
        {
            return true;
        }

        private void ExportSelectionToGraphml()
        {
            var people = PersonListViewModel.SelectedPeople.Objects.ToList();

            var personIds = people
                .Select(p => p.Id)
                .ToList();

            var predicates = new List<Expression<Func<PersonAssociation, bool>>>();
            predicates.Add(p => personIds.Contains(p.SubjectPersonId));
            predicates.Add(p => personIds.Contains(p.ObjectPersonId));

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var personAssociations = unitOfWork.PersonAssociations.Find(predicates).ToList();
                _dataProvider.ExportDataToGraphML(people, personAssociations);
            }
        }

        private bool CanExportSelectionToGraphml()
        {
            return PersonListViewModel.SelectedPeople.Objects != null &&
                   PersonListViewModel.SelectedPeople.Objects.Any();
        }

        private void ImportPeople()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            _dataProvider.ImportData(dialog.FileName, false);
        }

        private bool CanImportPeople()
        {
            return true;
        }

        private void Exit()
        {
            throw new NotImplementedException();
        }

        private bool CanExit()
        {
            return true;
        }

        private void ShowOptionsDialog(object owner)
        {
            var dialogViewModel = new OptionsDialogViewModel(_dataProvider);
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private bool CanShowOptionsDialog(object owner)
        {
            return true;
        }
    }
}
