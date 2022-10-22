using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
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

        public RecordListViewModel RecordListViewModel { get; private set; }

        public RecordPropertiesViewModel RecordPropertiesViewModel { get; private set; }
        public RecordAssociationsViewModel RecordAssociationsViewModel { get; private set; }

        public LogViewModel LogViewModel { get; private set; }

        private RelayCommand<object> _createPersonCommand;
        private RelayCommand<object> _showOptionsDialogCommand;
        private RelayCommand _deleteSelectedPeopleCommand;
        private RelayCommand _exportPeopleCommand;
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
            IDialogService applicationDialogService,
            ILogger logger)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;

            LogViewModel = new LogViewModel();
            _logger = new LoggerDecorator(logger, LogViewModel);
            _dataProvider.Initialize(_logger);

            RecordListViewModel = new RecordListViewModel(dataProvider, applicationDialogService);

            RecordListViewModel.SelectedPeople.PropertyChanged += HandlePeopleSelectionChanged;

            RecordPropertiesViewModel = new RecordPropertiesViewModel(
                dataProvider,
                RecordListViewModel.SelectedPeople);

            RecordAssociationsViewModel = new RecordAssociationsViewModel(
                dataProvider,
                applicationDialogService,
                RecordListViewModel.SelectedPeople);
        }

        private void HandlePeopleSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            DeleteSelectedPeopleCommand.RaiseCanExecuteChanged();
        }

        private void CreatePerson(object owner)
        {
            var dialogViewModel = new CreateRecordDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            _dataProvider.CreateRecord(new Record
            {
                Term = dialogViewModel.Term,
                Source = dialogViewModel.Source,
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
            _dataProvider.DeleteRecords(RecordListViewModel.SelectedPeople.Objects.ToList());
        }

        private bool CanDeleteSelectedPeople()
        {
            return RecordListViewModel.SelectedPeople.Objects != null &&
                   RecordListViewModel.SelectedPeople.Objects.Any();
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
