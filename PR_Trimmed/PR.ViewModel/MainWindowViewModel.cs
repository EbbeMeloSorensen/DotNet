using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using PR.Application;
using PR.Domain.Entities.PR;
using PR.IO;
using PR.Persistence;

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
                PersonPropertiesViewModel.UnitOfWorkFactory = value;
            }
        }

        public PersonListViewModel PersonListViewModel { get; }
        public PeoplePropertiesViewModel PeoplePropertiesViewModel { get; }
        public PersonPropertiesViewModel PersonPropertiesViewModel { get; }
        public LogViewModel LogViewModel { get; }

        private AsyncCommand<object> _createPersonCommand;
        private RelayCommand<object> _showOptionsDialogCommand;
        private AsyncCommand _deleteSelectedPeopleCommand;
        private AsyncCommand<object> _clearRepositoryCommand;
        private AsyncCommand _exportPeopleCommand;
        private RelayCommand _exportSelectionToGraphmlCommand;
        private AsyncCommand _importPeopleCommand;
        private RelayCommand _exitCommand;

        public AsyncCommand DeleteSelectedPeopleCommand
        {
            get { return _deleteSelectedPeopleCommand ?? (_deleteSelectedPeopleCommand = new AsyncCommand(DeleteSelectedPeople, CanDeleteSelectedPeople)); }
        }

        public AsyncCommand<object> ClearRepositoryCommand
        {
            get
            {
                return _clearRepositoryCommand ?? (_clearRepositoryCommand =
                    new AsyncCommand<object>(ClearRepository, CanClearRepository));
            }
        }

        public AsyncCommand<object> CreatePersonCommand
        {
            get { return _createPersonCommand ?? (_createPersonCommand = new AsyncCommand<object>(CreatePerson, CanCreatePerson)); }
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

            PersonPropertiesViewModel = new PersonPropertiesViewModel(
                unitOfWorkFactory,
                applicationDialogService,
                PersonListViewModel.SelectedPeople);

            PeoplePropertiesViewModel.PeopleUpdated += PeoplePropertiesViewModel_PeopleUpdated;

            _logger.WriteLine(LogMessageCategory.Information, "Application started");
        }

        public void Initialize(
            bool versioned,
            bool reseeding)
        {
            UnitOfWorkFactory.Initialize(versioned);

            if (reseeding)
            {
                UnitOfWorkFactory.Reseed();
            }

            if (UnitOfWorkFactory is IUnitOfWorkFactoryHistorical unitOfWorkFactoryHistorical)
            {
                unitOfWorkFactoryHistorical.IncludeCurrentObjects = true;
                unitOfWorkFactoryHistorical.IncludeHistoricalObjects = false;
            }
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

        private async Task CreatePerson(
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
                Nickname = dialogViewModel.Nickname,
                Address = dialogViewModel.Address,
                ZipCode = dialogViewModel.ZipCode,
                City = dialogViewModel.City,
                Birthday = dialogViewModel.Birthday,
                Category = dialogViewModel.Category,
                Start = dialogViewModel.Start.HasValue ? dialogViewModel.Start.Value : new DateTime(),
                End = dialogViewModel.End.HasValue ? dialogViewModel.End.Value : new DateTime()
            };

            using (var unitOfWork = _application.UnitOfWorkFactory.GenerateUnitOfWork())
            {
                await unitOfWork.People.Add(person);
                unitOfWork.Complete();
            }

            if (person.End > DateTime.UtcNow)
            {
                PersonListViewModel.AddPerson(person);
            }
        }

        private bool CanCreatePerson(
            object owner)
        {
            return true;
        }

        private async Task DeleteSelectedPeople()
        {
            // Denne wrapping skal helst undgås. I øvrigt er det problematisk mht at opdatere brugergrænsefladen
            //await Task.Run(async () =>
            //{
                using var unitOfWork = _application.UnitOfWorkFactory.GenerateUnitOfWork();
                var ids = PersonListViewModel.SelectedPeople.Objects.Select(p => p.ID).ToList();

                var peopleForDeletion = (await unitOfWork.People
                        .Find(pa => ids.Contains(pa.ID)))
                    .ToList();

                await unitOfWork.People.RemoveRange(peopleForDeletion);
                unitOfWork.Complete();

                PersonListViewModel.RemovePeople(peopleForDeletion);
            //});
        }

        private bool CanDeleteSelectedPeople()
        {
            return PersonListViewModel.SelectedPeople.Objects != null &&
                   PersonListViewModel.SelectedPeople.Objects.Any() &&
                   PersonListViewModel.SelectedPeople.Objects.All(_ => _.End.Year == 9999);
        }

        private async Task ClearRepository(
            object owner)
        {
            var dialogViewModel1 = new MessageBoxDialogViewModel("Clear repository?", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel1, owner as Window) == DialogResult.Cancel)
            {
                return;
            }

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                await unitOfWork.PersonComments.Clear();
                await unitOfWork.People.Clear();
                unitOfWork.Complete();
            }

            //_historicalChangeTimes.Clear();
            //UpdateHistoricalTimeSeriesView(false);

            //_databaseWriteTimes.Clear();
            //UpdateDatabaseTimeSeriesView();

            //await AutoFindIfEnabled();

            var dialogViewModel2 = new MessageBoxDialogViewModel("Repository was cleared", false);

            _applicationDialogService.ShowDialog(dialogViewModel2, owner as Window);
        }

        private bool CanClearRepository(
            object owner)
        {
            return true;
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
                .Select(p => p.ID)
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
            DateTime? historicalTime = null;
            DateTime? databaseTime = null;

            if (UnitOfWorkFactory is IUnitOfWorkFactoryVersioned unitOfWorkFactoryVersioned)
            {
                databaseTime = unitOfWorkFactoryVersioned.DatabaseTime;
            }

            if (UnitOfWorkFactory is IUnitOfWorkFactoryHistorical unitOfWorkFactoryHistorical)
            {
                historicalTime = unitOfWorkFactoryHistorical.HistoricalTime;
            }

            var dialogViewModel = new OptionsDialogViewModel(
                historicalTime,
                databaseTime);

            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);

            if (UnitOfWorkFactory is IUnitOfWorkFactoryVersioned unitOfWorkFactoryVersioned2)
            {
                unitOfWorkFactoryVersioned2.DatabaseTime = dialogViewModel.DatabaseTime;
            }

            if (UnitOfWorkFactory is IUnitOfWorkFactoryHistorical unitOfWorkFactoryHistorical2)
            {
                unitOfWorkFactoryHistorical2.HistoricalTime = dialogViewModel.HistoricalTime;
            }
        }

        private bool CanShowOptionsDialog(
            object owner)
        {
            return true;
        }
    }
}
