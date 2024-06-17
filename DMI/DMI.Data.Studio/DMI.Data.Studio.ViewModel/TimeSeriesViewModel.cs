using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DMI.SMS.Domain.Entities;
using GalaSoft.MvvmLight;
using Craft.Logging;
using Craft.Utils;

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
        private readonly ObjectCollection<StationInformation> _selectedStationInformations;
        private DateTime _timeAtOrigo;
        private TimeSpan _timeSpanForXUnit = TimeSpan.FromDays(1);
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
            ObjectCollection<StationInformation> selectedStationInformations)
        {
            _obsDBUnitOfWorkFactory = obsDBUnitOfWorkFactory;
            _selectedStationInformations = selectedStationInformations;

            var timeWindow = TimeSpan.FromDays(7);
            var utcNow = DateTime.UtcNow;
            _timeAtOrigo = utcNow.Date - TimeSpan.FromDays(7);
            //_timeAtOrigo = new DateTime(2022, _timeAtOrigo.Month, _timeAtOrigo.Day, 0, 0, 0, DateTimeKind.Utc);
            var tFocus = _timeAtOrigo + timeWindow / 2;
            var xFocus = (tFocus - _timeAtOrigo) / TimeSpan.FromDays(1.0);

            ScatterChartViewModel = new Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel(
                new Point(xFocus, 10),
                new Size(14, 30),
                true,
                25,
                60,
                1.0,
                _timeAtOrigo,
                null);

            ScatterChartViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured +=
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;

            _selectedStationInformations.PropertyChanged += _selectedStationInformations_PropertyChanged;
        }

        private void _selectedStationInformations_PropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();

            var stationInformations = sender as ObjectCollection<StationInformation>;

            if (stationInformations == null || stationInformations.Objects.Count() == 0)
            {
                _nanoqStationId = null;
            }
            else if (stationInformations.Objects.Count() == 1)
            {
                _nanoqStationId = $"{stationInformations.Objects.Single().StationIDDMI}00";
            }

            UpdateCurve();
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            Craft.ViewModels.Geometry2D.ScrollFree.WorldWindowUpdatedEventArgs e)
        {
            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();

            var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

            _t0 = _timeAtOrigo + x0 * (_timeSpanForXUnit);
            _t1 = _timeAtOrigo + x1 * (_timeSpanForXUnit);

            UpdateCurve();
        }

        private void UpdateCurve()
        {
            if (_t1 <= _t0)
            {
                return;
            }

            Logger?.WriteLine(LogMessageCategory.Information, "TimeSeries view:");

            if (_nanoqStationId == null)
            {
                return;
            }

            using (var unitOfWork = _obsDBUnitOfWorkFactory.GenerateUnitOfWork())
            {
                var statId = int.Parse(_nanoqStationId);

                Logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving station {_nanoqStationId}..", "general", true);

                var observingFacility = unitOfWork.ObservingFacilities
                    .Find(_ => _.StatId == statId)
                    .SingleOrDefault();

                Logger?.WriteLine(LogMessageCategory.Information, "    done", "general", false);

                if (observingFacility == null)
                {
                    return;
                }

                Logger?.WriteLine(LogMessageCategory.Information, "  Retrieving timeseries..", "general", true);

                observingFacility = unitOfWork.ObservingFacilities
                    .GetIncludingTimeSeries(observingFacility.Id);

                if (observingFacility.TimeSeries == null)
                {
                    return;
                }

                var timeSeries = observingFacility.TimeSeries
                    .Where(_ => _.ParamId == "temp_dry")
                    .SingleOrDefault();

                Logger?.WriteLine(LogMessageCategory.Information, "    done", "general", false);

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
                    var x = (t - _timeAtOrigo) / _timeSpanForXUnit;

                    // Add the point to the polyline
                    points.Add(new PointD(x, observation.Value));
                }

                ScatterChartViewModel.GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
            }
        }
    }
}
