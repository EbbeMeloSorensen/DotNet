using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Craft.Logging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D;
using Craft.ViewModels.Tasks;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application.Application _application;
        private readonly IDialogService _applicationDialogService;
        private readonly Brush _pointBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly ObservableObject<bool> _observableForClassifyRecordsWithCondition;
        private string _mainWindowTitle;
        private bool _classifyRecordsWithCondition;

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                _mainWindowTitle = value;
                RaisePropertyChanged();
            }
        }

        public bool ClassifyRecordsWithCondition
        {
            get { return _classifyRecordsWithCondition; }
            set
            {
                _classifyRecordsWithCondition = value;
                RaisePropertyChanged();

                _observableForClassifyRecordsWithCondition.Object = value;
            }
        }

        public RelayCommand<object> CreateStationInformationCommand { get; }
        public RelayCommand DeleteSelectedStationInformationsCommand { get; }
        public AsyncCommand<object> ExportDataCommand { get; }
        public AsyncCommand<object> ImportDataCommand { get; }
        public AsyncCommand<object> MakeBreakfastCommand { get; }
        public AsyncCommand<object> ExtractMeteorologicalStationsCommand { get; }
        public AsyncCommand<object> ExtractOceanographicalStationsCommand { get; }
        public RelayCommand<object> OpenSettingsDialogCommand { get; }

        public StationInformationListViewModel StationInformationListViewModel { get; }
        public StationInformationDetailsViewModel StationInformationDetailsViewModel { get; }
        public StationInformationCollectionViewModel StationInformationCollectionViewModel { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }
        public TaskViewModel TaskViewModel { get; }
        public LogViewModel LogViewModel { get; }

        public MainWindowViewModel(
            Application.Application application,
            IDialogService applicationDialogService)
        {
            _application = application;
            _applicationDialogService = applicationDialogService;

            LogViewModel = new LogViewModel();

            _application.Logger = new ViewModelLogger(_application.Logger, LogViewModel);
            //_application.Logger = null; // Set to null to disable logging

            _application.Initialize();

            _mainWindowTitle = "SMS Studio";

            _observableForClassifyRecordsWithCondition = new ObservableObject<bool>();
            _observableForClassifyRecordsWithCondition.Object = true;

            StationInformationListViewModel = new StationInformationListViewModel(
                _application.UIDataProvider,
                applicationDialogService,
                _observableForClassifyRecordsWithCondition);

            StationInformationDetailsViewModel = new StationInformationDetailsViewModel(
                _application.UIDataProvider,
                applicationDialogService,
                StationInformationListViewModel.SelectedStationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            StationInformationCollectionViewModel = new StationInformationCollectionViewModel(
                _application.UIDataProvider,
                StationInformationListViewModel.SelectedStationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            _classifyRecordsWithCondition = true;

            StationInformationCollectionViewModel.ImageWidth = 1200;
            StationInformationCollectionViewModel.ImageHeight = 900;

            StationInformationListViewModel.SelectedStationInformations.PropertyChanged += SelectedStationInformations_PropertyChanged;

            GeometryEditorViewModel = new GeometryEditorViewModel();
            GeometryEditorViewModel.Magnification = 240;
            GeometryEditorViewModel.WorldWindowUpperLeft = new System.Windows.Point(7.4, 53.9);

            TaskViewModel = new TaskViewModel();

            CreateStationInformationCommand = new RelayCommand<object>(CreateStationInformation);
            DeleteSelectedStationInformationsCommand = new RelayCommand(DeleteSelectedStationInformations, CanDeleteSelectedStationInformations);
            ExportDataCommand = new AsyncCommand<object>(ExportData, CanExportData);
            ImportDataCommand = new AsyncCommand<object>(ImportData, CanImportData);
            MakeBreakfastCommand = new AsyncCommand<object>(MakeBreakfast, CanMakeBreakfast);
            ExportDataCommand = new AsyncCommand<object>(ExportData, CanExportData);
            ImportDataCommand = new AsyncCommand<object>(ImportData, CanImportData);
            ExtractMeteorologicalStationsCommand = new AsyncCommand<object>(ExtractMeteorologicalStations, CanExtractMeteorologicalStations);
            ExtractOceanographicalStationsCommand = new AsyncCommand<object>(ExtractOceanographicalStations, CanExtractOceanographicalStations);
            OpenSettingsDialogCommand = new RelayCommand<object>(OpenSettingsDialog);

            var lineThickness = 0.02;

            var fyn_p1 = new Point2D(9.8, 55.36);
            var fyn_p2 = new Point2D(10.31, 55.62);
            var fyn_p3 = new Point2D(10.83, 55.23);
            var fyn_p4 = new Point2D(10.3, 55.05);
            GeometryEditorViewModel.AddLine(fyn_p1, fyn_p2, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p2, fyn_p3, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p3, fyn_p4, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p4, fyn_p1, lineThickness);

            var jylland_p1 = new Point2D(8.65, 54.92);
            var jylland_p2 = new Point2D(8.08, 55.57);
            var jylland_p3 = new Point2D(8.13, 56.56);
            var jylland_p4 = new Point2D(8.61, 57.12);
            var jylland_p5 = new Point2D(9.61, 57.27);
            var jylland_p6 = new Point2D(10.62, 57.74);
            var jylland_p7 = new Point2D(10.43, 56.52);
            var jylland_p8 = new Point2D(10.89, 56.49);
            var jylland_p9 = new Point2D(10.76, 56.17);
            var jylland_p10 = new Point2D(10.33, 56.26);
            var jylland_p11 = new Point2D(10.2, 55.83);
            var jylland_p12 = new Point2D(9.6, 55.42);
            var jylland_p13 = new Point2D(9.46, 54.84);
            GeometryEditorViewModel.AddLine(jylland_p1, jylland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p2, jylland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p3, jylland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p4, jylland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p5, jylland_p6, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p6, jylland_p7, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p7, jylland_p8, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p8, jylland_p9, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p9, jylland_p10, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p10, jylland_p11, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p11, jylland_p12, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p12, jylland_p13, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p13, jylland_p1, lineThickness);

            var sjalland_p1 = new Point2D(10.98, 55.74);
            var sjalland_p2 = new Point2D(12.26, 56.14);
            var sjalland_p3 = new Point2D(12.67, 55.6);
            var sjalland_p4 = new Point2D(12.05, 54.98);
            var sjalland_p5 = new Point2D(11.22, 55.2);
            GeometryEditorViewModel.AddLine(sjalland_p1, sjalland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p2, sjalland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p3, sjalland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p4, sjalland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p5, sjalland_p1, lineThickness);
        }

        private void CreateStationInformation(
            object owner)
        {
            var dialogViewModel = new CreateStationInformationDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

            _application.UIDataProvider.CreateStationInformation(new StationInformation
            {
                StationName = dialogViewModel.StationName,
                GdbFromDate = currentTime,
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59)
            }, true);
        }

        public void DeleteSelectedStationInformations()
        {
            throw new NotImplementedException();
            //_dataProvider.DeletePeople(PersonListViewModel.SelectedPeople.Objects.ToList());
        }

        private bool CanDeleteSelectedStationInformations()
        {
            return StationInformationListViewModel.SelectedStationInformations.Objects != null &&
                   StationInformationListViewModel.SelectedStationInformations.Objects.Any();
        }

        //private void ExportData()
        //{
        //    //var dialog = new SaveFileDialog
        //    //{
        //    //    Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
        //    //};

        //    //if (dialog.ShowDialog() == false)
        //    //{
        //    //    return;
        //    //}

        //    //_dataProvider.ExportPeople(dialog.FileName);
        //    _application.UIDataProvider.ExportData(@"C:\Temp\SMSData.xml");
        //}

        //private bool CanExportData()
        //{
        //    return true;
        //}

        //private void ImportData()
        //{
        //    //var dialog = new OpenFileDialog
        //    //{
        //    //    Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
        //    //};

        //    //if (dialog.ShowDialog() == false)
        //    //{
        //    //    return;
        //    //}

        //    _application.UIDataProvider.ImportData(@"C:\Temp\SMSData.xml");
        //}

        //private bool CanImportData()
        //{
        //    return true;
        //}

        private async Task ExportData(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Export data", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            TaskViewModel.NameOfTask = "Exporting data";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.ExportData(
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            TaskViewModel.Busy = false;
            RefreshCommandAvailability();

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed exporting data", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }
        }

        private bool CanExportData(
            object owner)
        {
            return !TaskViewModel.Busy;
        }

        private async Task ImportData(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Import Data", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            TaskViewModel.NameOfTask = "Importing data";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.ExportData(
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            TaskViewModel.Busy = false;
            RefreshCommandAvailability();

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed importing data", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }
        }

        private bool CanImportData(
            object owner)
        {
            return !TaskViewModel.Busy;
        }

        private async Task MakeBreakfast(
            object owner)
        {
            var dialogViewModel = new ExtractFrieDataStationListDialogViewModel(
                "Make Breakfast");

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            DateTime? dateTime = null;

            if (!string.IsNullOrEmpty(dialogViewModel.Date))
            {
                dialogViewModel.Date.TryParsingAsDateTime(out var temp);
                dateTime = temp;
            }

            TaskViewModel.NameOfTask = "Making breakfast";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.MakeBreakfast(
                dateTime,
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            TaskViewModel.Busy = false;
            RefreshCommandAvailability();

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed breakfast", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }
        }

        private bool CanMakeBreakfast(
            object owner)
        {
            return !TaskViewModel.Busy;
        }

        private async Task ExtractMeteorologicalStations(
            object owner)
        {
            var dialogViewModel = new ExtractFrieDataStationListDialogViewModel(
                "Extract Meteorological Stations");

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            DateTime? dateTime = null;

            if (!string.IsNullOrEmpty(dialogViewModel.Date))
            {
                dialogViewModel.Date.TryParsingAsDateTime(out var temp);
                dateTime = temp;
            }

            TaskViewModel.NameOfTask = "Extracting Meteorological Stations";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.ExtractMeteorologicalStations(
                dateTime,
                
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            TaskViewModel.Busy = false;
            RefreshCommandAvailability();

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed Extraction of Meteorological Stations", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }
        }

        private bool CanExtractMeteorologicalStations(
            object owner)
        {
            return !TaskViewModel.Busy;
        }

        private async Task ExtractOceanographicalStations(
            object owner)
        {
            var dialogViewModel = new ExtractFrieDataStationListDialogViewModel(
                "Extract Oceanographical Stations");

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            DateTime? dateTime = null;

            if (!string.IsNullOrEmpty(dialogViewModel.Date))
            {
                dialogViewModel.Date.TryParsingAsDateTime(out var temp);
                dateTime = temp;
            }

            TaskViewModel.NameOfTask = "Extracting Oceanographical Stations";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.MakeBreakfast(
                dateTime,
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            TaskViewModel.Busy = false;
            RefreshCommandAvailability();

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed Extraction of Oceanographical Stations", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }
        }

        private bool CanExtractOceanographicalStations(
            object owner)
        {
            return !TaskViewModel.Busy;
        }

        private void OpenSettingsDialog(
            object owner)
        {
            var dialogViewModel = new SettingsDialogViewModel(
                _application.UIDataProvider);

            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private void SelectedStationInformations_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            var stationInformations = sender as ObjectCollection<StationInformation>;

            GeometryEditorViewModel.ClearPoints();

            if (stationInformations == null || !stationInformations.Objects.Any())
            {
                return;
            }

            foreach (var stationInformation in stationInformations.Objects)
            {
                if (!stationInformation.Wgs_lat.HasValue ||
                    !stationInformation.Wgs_long.HasValue)
                {
                    continue;
                }

                var point = new Point2D(
                    stationInformation.Wgs_long.Value,
                    stationInformation.Wgs_lat.Value);

                GeometryEditorViewModel.AddPoint(point, 20, _pointBrush);
            }
        }

        private void RefreshCommandAvailability()
        {
            ExportDataCommand.RaiseCanExecuteChanged();
            ImportDataCommand.RaiseCanExecuteChanged();
            MakeBreakfastCommand.RaiseCanExecuteChanged();
            ExtractMeteorologicalStationsCommand.RaiseCanExecuteChanged();
            ExtractOceanographicalStationsCommand.RaiseCanExecuteChanged();
        }
    }
}
