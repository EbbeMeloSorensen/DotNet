using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DMI.SMS.Domain.Entities;
using DMI.StatDB.Domain.Entities;
using GalaSoft.MvvmLight;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;

namespace DMI.Data.Studio.ViewModel
{
    // Denne klasse:
    //   - HAR en TimeSeriesViewModel - ja det hedder det samme for now
    //   - BESTEMMER, hvad tidspunkt origo (x = 0) svarer til, samt hvad x = 1 svarer til
    //   - BESTEMMER initiel position af World Window (efterfølgende kommunikeres brugerens justeringer af WorldWindow fra ScatterChartViewModel)
    //   Når der sker en "major" opdatering af World Window, så hentes nye tidsseriedata fra datakilden
    public class TimeSeriesViewModel : ViewModelBase
    {
        private ILogger _logger;
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 1.0;
        private readonly ObsDB.Persistence.IUnitOfWorkFactory _obsDBUnitOfWorkFactory;
        private readonly ObjectCollection<Station> _selectedStations;
        private string _nanoqStationId;
        private DateTime _t0;
        private DateTime _t1;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel ScatterChartViewModel { get; set; }

        public TimeSeriesViewModel(
            ObsDB.Persistence.IUnitOfWorkFactory obsDBUnitOfWorkFactory,
            ObjectCollection<Station> selectedStations)
        {
            _obsDBUnitOfWorkFactory = obsDBUnitOfWorkFactory;
            _selectedStations = selectedStations;

            var timeWindow = TimeSpan.FromDays(60);
            var utcNow = DateTime.UtcNow;
            var tFocus = utcNow.Date - timeWindow / 2;
            var xFocus = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(tFocus);

            ScatterChartViewModel = new Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel(
                new Point(xFocus, 10),
                new Size(timeWindow.TotalDays, 30),
                true,
                0,
                60,
                1.0,
                XAxisMode.Cartesian,
                null);

            ScatterChartViewModel.GeometryEditorViewModel.YAxisLocked = false;
            ScatterChartViewModel.ShowPanningButtons = true;
            ScatterChartViewModel.ShowYAxisLabels = false;

            ScatterChartViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured +=
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;

            ScatterChartViewModel.PanLeftClicked += (s, e) =>
            {
                ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft = new Point(
                    ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X - ScatterChartViewModel.GeometryEditorViewModel.WorldWindowSize.Width,
                    ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y);
            };

            ScatterChartViewModel.PanRightClicked += (s, e) =>
            {
                ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft = new Point(
                    ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X + ScatterChartViewModel.GeometryEditorViewModel.WorldWindowSize.Width,
                    ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y);
            };

            _selectedStations.PropertyChanged += _selectedStations_PropertyChanged;
        }

        private void _selectedStations_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();

            var stations = sender as ObjectCollection<Station>;

            if (stations == null || stations.Objects.Count() != 1)
            {
                _nanoqStationId = null;
            }
            else
            {
                _nanoqStationId = $"{stations.Objects.Single().StatID}";
            }

            UpdateCurve();
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();

            var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

            _t0 = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertXValueToDateTime(x0);
            _t1 = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertXValueToDateTime(x1);

            UpdateCurve();
        }

        private void UpdateCurve()
        {
            if (_t1 <= _t0)
            {
                return;
            }

            //Logger?.WriteLine(LogMessageCategory.Information, "TimeSeries view:");

            if (_nanoqStationId == null)
            {
                return;
            }

            using (var unitOfWork = _obsDBUnitOfWorkFactory.GenerateUnitOfWork())
            {
                var statId = int.Parse(_nanoqStationId);

                //Logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving station {_nanoqStationId}..", "general", true);

                var observingFacility = unitOfWork.ObservingFacilities
                    .Find(_ => _.StatId == statId)
                    .SingleOrDefault();

                //Logger?.WriteLine(LogMessageCategory.Information, "    done", "general", false);

                if (observingFacility == null)
                {
                    return;
                }

                //Logger?.WriteLine(LogMessageCategory.Information, "  Retrieving timeseries..", "general", true);

                observingFacility = unitOfWork.ObservingFacilities
                    .GetIncludingTimeSeries(observingFacility.Id);

                if (observingFacility.TimeSeries == null)
                {
                    return;
                }

                var timeSeries = observingFacility.TimeSeries
                    .Where(_ => _.ParamId == "temp_dry")
                    .SingleOrDefault();

                //Logger?.WriteLine(LogMessageCategory.Information, "    done", "general", false);

                if (timeSeries == null)
                {
                    return;
                }

                Logger?.WriteLine(LogMessageCategory.Information, "  Retrieving observations..", "general", true);

                timeSeries = unitOfWork.TimeSeries.GetIncludingObservations(
                    timeSeries.Id, _t0, _t1);

                Logger?.WriteLine(LogMessageCategory.Information, "    done", "general", false);

                Logger?.WriteLine(LogMessageCategory.Information,
                    $"    (retrieved {timeSeries.Observations.Count} observations)", "general", false);

                if (timeSeries.Observations == null)
                {
                    return;
                }

                var points = new List<PointD>();

                foreach (var observation in timeSeries.Observations)
                {
                    var t = observation.Time;

                    // Find the x coordinate that corresponds to the current time
                    var x = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(t);

                    // Add the point to the polyline
                    points.Add(new PointD(x, observation.Value));
                }

                ScatterChartViewModel.GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
            }
        }
    }
}
