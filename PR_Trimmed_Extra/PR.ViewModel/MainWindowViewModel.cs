using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PR.Application;
using PR.Domain.Entities;
using PR.IO;
using PR.Persistence;
using static System.Net.Mime.MediaTypeNames;

namespace PR.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application.Application _application;
        private readonly IDataIOHandler _dataIOHandler;
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

        public IUnitOfWorkFactory UnitOfWorkFactory
        {
            get => _application.UnitOfWorkFactory;
            set
            {
                _application.UnitOfWorkFactory = value;
                PersonListViewModel.UnitOfWorkFactory = value;
                PeoplePropertiesViewModel.UnitOfWorkFactory = value;
            }
        }

        public PersonListViewModel PersonListViewModel { get; }
        public PeoplePropertiesViewModel PeoplePropertiesViewModel { get; }
        public LogViewModel LogViewModel { get; }

        private RelayCommand<object> _createPersonCommand;
        private RelayCommand<object> _showOptionsDialogCommand;
        private RelayCommand _deleteSelectedPeopleCommand;
        private AsyncCommand _exportPeopleCommand;
        private RelayCommand _exportSelectionToGraphmlCommand;
        private AsyncCommand _importPeopleCommand;
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

        public AsyncCommand ExportPeopleCommand
        {
            get { return _exportPeopleCommand ?? (_exportPeopleCommand = new AsyncCommand(ExportPeople, CanExportPeople)); }
        }

        public RelayCommand ExportSelectionToGraphmlCommand
        {
            get { return _exportSelectionToGraphmlCommand ?? (_exportSelectionToGraphmlCommand = new RelayCommand(
                ExportSelectionToGraphml, CanExportSelectionToGraphml)); }
        }

        public AsyncCommand ImportPeopleCommand
        {
            get { return _importPeopleCommand ?? (_importPeopleCommand = new AsyncCommand(ImportPeople, CanImportPeople)); }
        }

        public RelayCommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(Exit, CanExit)); }
        }

        public MainWindowViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler,
            IDialogService applicationDialogService,
            ILogger logger)
        {
            _application = new Application.Application(
                unitOfWorkFactory, 
                dataIOHandler, 
                logger);

            _application.UnitOfWorkFactory = unitOfWorkFactory;
            _dataIOHandler = dataIOHandler;
            _applicationDialogService = applicationDialogService;

            LogViewModel = new LogViewModel(200);
            _logger = new ViewModelLogger(logger, LogViewModel);

            PersonListViewModel = new PersonListViewModel(unitOfWorkFactory, applicationDialogService);

            PersonListViewModel.SelectedPeople.PropertyChanged += HandlePeopleSelectionChanged;

            PeoplePropertiesViewModel = new PeoplePropertiesViewModel(
                unitOfWorkFactory,
                PersonListViewModel.SelectedPeople);

            PeoplePropertiesViewModel.PeopleUpdated += PeoplePropertiesViewModel_PeopleUpdated;

            _logger.WriteLine(LogMessageCategory.Information, "Application started");
        }

        private void PeoplePropertiesViewModel_PeopleUpdated(
            object? sender, 
            PeopleEventArgs e)
        {
            PersonListViewModel.UpdatePeople(e.People);
        }

        private void HandlePeopleSelectionChanged(
            object sender, 
            PropertyChangedEventArgs e)
        {
            DeleteSelectedPeopleCommand.RaiseCanExecuteChanged();
            ExportSelectionToGraphmlCommand.RaiseCanExecuteChanged();
        }

        private void CreatePerson(
            object owner)
        {
            var dialogViewModel = new CreatePersonDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var person = new Person
            {
                FirstName = dialogViewModel.FirstName,
                Surname = dialogViewModel.Surname,
                Created = DateTime.UtcNow
            };

            using (var unitOfWork = _application.UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.Add(person);
                unitOfWork.Complete();
            }

            PersonListViewModel.AddPerson(person);
        }

        private bool CanCreatePerson(
            object owner)
        {
            return true;
        }

        private void DeleteSelectedPeople()
        {
            using (var unitOfWork = _application.UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = PersonListViewModel.SelectedPeople.Objects.Select(p => p.Id).ToList();

                var peopleForDeletion = unitOfWork.People
                    .Find(pa => ids.Contains(pa.Id))
                    .ToList();

                unitOfWork.People.RemoveRange(peopleForDeletion);
                unitOfWork.Complete();

                PersonListViewModel.RemovePeople(peopleForDeletion);
            }
        }

        private bool CanDeleteSelectedPeople()
        {
            return PersonListViewModel.SelectedPeople.Objects != null &&
                   PersonListViewModel.SelectedPeople.Objects.Any();
        }

        private async Task ExportPeople()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            await _application.ExportData(dialog.FileName);
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

            var prData = new PRData
            {
                People = people,
            };

            _dataIOHandler.ExportDataToGraphML(
                prData,
                @"C:\Temp\People.graphml");
        }

        private bool CanExportSelectionToGraphml()
        {
            return PersonListViewModel.SelectedPeople.Objects != null &&
                   PersonListViewModel.SelectedPeople.Objects.Any();
        }

        private async Task ImportPeople()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            await _application.ImportData(dialog.FileName);
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

        private void ShowOptionsDialog(
            object owner)
        {
            var dialogViewModel = new OptionsDialogViewModel();
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private bool CanShowOptionsDialog(
            object owner)
        {
            return true;
        }
    }
}
