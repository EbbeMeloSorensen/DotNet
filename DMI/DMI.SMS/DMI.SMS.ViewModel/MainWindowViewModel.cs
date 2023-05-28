using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Tasks;
using DMI.SMS.Domain.Entities;
using System.Globalization;

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
        public AsyncCommand<object> ClearRepositoryCommand { get; }
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
            ClearRepositoryCommand = new AsyncCommand<object>(ClearRepository, CanClearRepository);
            MakeBreakfastCommand = new AsyncCommand<object>(MakeBreakfast, CanMakeBreakfast);
            ExtractMeteorologicalStationsCommand = new AsyncCommand<object>(ExtractMeteorologicalStations, CanExtractMeteorologicalStations);
            ExtractOceanographicalStationsCommand = new AsyncCommand<object>(ExtractOceanographicalStations, CanExtractOceanographicalStations);
            OpenSettingsDialogCommand = new RelayCommand<object>(OpenSettingsDialog);

            var lineThickness = 0.02;
            var brush = new SolidColorBrush(Colors.Black);

            var fyn_p1 = new PointD(9.8, 55.36);
            var fyn_p2 = new PointD(10.31, 55.62);
            var fyn_p3 = new PointD(10.83, 55.23);
            var fyn_p4 = new PointD(10.3, 55.05);
            GeometryEditorViewModel.AddLine(fyn_p1, fyn_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p2, fyn_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p3, fyn_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p4, fyn_p1, lineThickness, brush);

            var jylland_p1 = new PointD(8.65, 54.92);
            var jylland_p2 = new PointD(8.08, 55.57);
            var jylland_p3 = new PointD(8.13, 56.56);
            var jylland_p4 = new PointD(8.61, 57.12);
            var jylland_p5 = new PointD(9.61, 57.27);
            var jylland_p6 = new PointD(10.62, 57.74);
            var jylland_p7 = new PointD(10.43, 56.52);
            var jylland_p8 = new PointD(10.89, 56.49);
            var jylland_p9 = new PointD(10.76, 56.17);
            var jylland_p10 = new PointD(10.33, 56.26);
            var jylland_p11 = new PointD(10.2, 55.83);
            var jylland_p12 = new PointD(9.6, 55.42);
            var jylland_p13 = new PointD(9.46, 54.84);
            GeometryEditorViewModel.AddLine(jylland_p1, jylland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p2, jylland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p3, jylland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p4, jylland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p5, jylland_p6, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p6, jylland_p7, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p7, jylland_p8, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p8, jylland_p9, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p9, jylland_p10, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p10, jylland_p11, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p11, jylland_p12, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p12, jylland_p13, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p13, jylland_p1, lineThickness, brush);

            var sjalland_p1 = new PointD(10.98, 55.74);
            var sjalland_p2 = new PointD(12.26, 56.14);
            var sjalland_p3 = new PointD(12.67, 55.6);
            var sjalland_p4 = new PointD(12.05, 54.98);
            var sjalland_p5 = new PointD(11.22, 55.2);
            GeometryEditorViewModel.AddLine(sjalland_p1, sjalland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p2, sjalland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p3, sjalland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p4, sjalland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p5, sjalland_p1, lineThickness, brush);
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

            var stationInformation = new StationInformation
            {
                StationName = dialogViewModel.StationName,
                StationIDDMI = int.Parse(dialogViewModel.Stationid_dmi),
                Stationtype = dialogViewModel.StationType.ConvertFromDisplayTextToStationType(),
                StationOwner = dialogViewModel.StationOwner.ConvertFromDisplayTextToStationOwner(),
                Country = dialogViewModel.Country.ConvertFromDisplayTextToCountry(),
                Status = dialogViewModel.Status.ConvertFromDisplayTextToStatus(),
                Wgs_lat = double.Parse(dialogViewModel.Wgs_lat, CultureInfo.InvariantCulture),
                Wgs_long = double.Parse(dialogViewModel.Wgs_long, CultureInfo.InvariantCulture),
                GdbFromDate = currentTime,
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };

            if (!string.IsNullOrEmpty(dialogViewModel.Hha))
            {
                stationInformation.Hha = double.Parse(dialogViewModel.Hha, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(dialogViewModel.Hhp))
            {
                stationInformation.Hhp = double.Parse(dialogViewModel.Hhp, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(dialogViewModel.DateFrom))
            {
                dialogViewModel.DateFrom.TryParsingAsDateTime(out var dateFrom);
                stationInformation.DateFrom = dateFrom;
            }

            if (!string.IsNullOrEmpty(dialogViewModel.DateTo))
            {
                dialogViewModel.DateTo.TryParsingAsDateTime(out var dateTo);
                stationInformation.DateTo = dateTo;
            }

            _application.UIDataProvider.CreateStationInformation(
                stationInformation, true);
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

        private async Task ExportData(
            object owner)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Json Files(*.json)|*.json|Xml Files(*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            TaskViewModel.NameOfTask = "Exporting data";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.ExportData(
                dialog.FileName,
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
                var messageBoxDialog = new MessageBoxDialogViewModel($"Exported data to {dialog.FileName}", false);
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
            var dialog = new OpenFileDialog
            {
                Filter = "Json Files(*.json)|*.json|Xml Files(*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            TaskViewModel.NameOfTask = "Importing data";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.ImportData(
                dialog.FileName,
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

        private async Task ClearRepository(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Clear Repository?", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            TaskViewModel.NameOfTask = "Clearing Repository";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.ClearRepository(
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
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed clearing repository", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }
        }

        private bool CanClearRepository(
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

            TaskViewModel.NameOfTask = "Making breakfast";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.MakeBreakfast(
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

            TaskViewModel.NameOfTask = "Extracting Oceanographical Stations";
            TaskViewModel.Abort = false;
            TaskViewModel.Busy = true;
            RefreshCommandAvailability();

            await _application.MakeBreakfast(
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

                var point = new PointD(
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
