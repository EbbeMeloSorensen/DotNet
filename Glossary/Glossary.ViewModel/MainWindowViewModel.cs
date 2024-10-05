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

        private RelayCommand<object> _createRecordCommand;
        private RelayCommand<object> _showOptionsDialogCommand;
        private RelayCommand _deleteSelectedRecordsCommand;
        private RelayCommand _exportRecordsCommand;
        private RelayCommand _importRecordsCommand;
        private RelayCommand _exitCommand;

        public RelayCommand DeleteSelectedRecordsCommand
        {
            get { return _deleteSelectedRecordsCommand ?? (_deleteSelectedRecordsCommand = new RelayCommand(DeleteSelectedRecords, CanDeleteSelectedRecords)); }
        }

        public RelayCommand<object> CreateRecordCommand
        {
            get { return _createRecordCommand ?? (_createRecordCommand = new RelayCommand<object>(CreateRecord, CanCreateRecord)); }
        }

        public RelayCommand<object> ShowOptionsDialogCommand
        {
            get { return _showOptionsDialogCommand ?? (_showOptionsDialogCommand = new RelayCommand<object>(ShowOptionsDialog, CanShowOptionsDialog)); }
        }

        public RelayCommand ExportRecordsCommand
        {
            get { return _exportRecordsCommand ?? (_exportRecordsCommand = new RelayCommand(ExportRecords, CanExportRecords)); }
        }

        public RelayCommand ImportRecordsCommand
        {
            get { return _importRecordsCommand ?? (_importRecordsCommand = new RelayCommand(ImportRecords, CanImportRecords)); }
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

            LogViewModel = new LogViewModel(100);
            _logger = new ViewModelLogger(logger, LogViewModel);
            _dataProvider.Initialize(_logger);

            RecordListViewModel = new RecordListViewModel(dataProvider, applicationDialogService);

            RecordListViewModel.SelectedRecords.PropertyChanged += HandleRecordSelectionChanged;

            RecordPropertiesViewModel = new RecordPropertiesViewModel(
                dataProvider,
                RecordListViewModel.SelectedRecords);

            RecordAssociationsViewModel = new RecordAssociationsViewModel(
                dataProvider,
                applicationDialogService,
                RecordListViewModel.SelectedRecords);
        }

        private void HandleRecordSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            DeleteSelectedRecordsCommand.RaiseCanExecuteChanged();
        }

        private void CreateRecord(object owner)
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

        private bool CanCreateRecord(object owner)
        {
            return true;
        }

        public void DeleteSelectedRecords()
        {
            _dataProvider.DeleteRecords(RecordListViewModel.SelectedRecords.Objects.ToList());
        }

        private bool CanDeleteSelectedRecords()
        {
            return RecordListViewModel.SelectedRecords.Objects != null &&
                   RecordListViewModel.SelectedRecords.Objects.Any();
        }

        private void ExportRecords()
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

        private bool CanExportRecords()
        {
            return true;
        }

        private void ImportRecords()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            _dataProvider.ImportData(dialog.FileName);
        }

        private bool CanImportRecords()
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
