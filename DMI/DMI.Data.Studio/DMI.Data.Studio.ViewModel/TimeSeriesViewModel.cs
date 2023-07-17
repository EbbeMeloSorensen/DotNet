using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DMI.SMS.Domain.Entities;
using GalaSoft.MvvmLight;
using Craft.Utils;
using DMI.ObsDB.Domain.Entities;

namespace DMI.Data.Studio.ViewModel
{
    // Denne klasse:
    //   - HAR en TimeSeriesViewModel - ja det hedder det samme for now
    //   - BESTEMMER, hvad tidspunkt origo (x = 0) svarer til, samt hvad x = 1 svarer til
    //   - BESTEMMER initiel position af World Window (efterfølgende kommunikeres brugerens justeringer af WorldWindow fra ScatterChartViewModel)
    //   Når der sker en "major" opdatering af World Window, så hentes nye tidsseriedata fra datakilden
    public class TimeSeriesViewModel : ViewModelBase
    {
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;
        private readonly ObsDB.Persistence.IUnitOfWorkFactory _obsDBUnitOfWorkFactory;
        private readonly ObjectCollection<StationInformation> _selectedStationInformations;
        private DateTime _timeAtOrigo;
        private TimeSpan _timeSpanForXUnit = TimeSpan.FromDays(1);
        private string _nanoqStationId;
        private string _parameter;
        private DateTime _t0;
        private DateTime _t1;

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
            _timeAtOrigo = new DateTime(1990, _timeAtOrigo.Month, _timeAtOrigo.Day);
            var tFocus = _timeAtOrigo + timeWindow / 2;
            var xFocus = (tFocus - _timeAtOrigo) / TimeSpan.FromDays(1.0);

            ScatterChartViewModel = new Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel(
                new Point(xFocus, 10),
                new Size(14, 30),
                true,
                25,
                60,
                _timeAtOrigo);

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
                _parameter = null;
            }
            else if (stationInformations.Objects.Count() == 1)
            {
                _nanoqStationId = $"{stationInformations.Objects.Single().StationIDDMI}00";
                _parameter = "temp_dry";
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
            if (_nanoqStationId == null)
            {
                return;
            }

            using (var unitOfWork = _obsDBUnitOfWorkFactory.GenerateUnitOfWork())
            {
                var predicates = new List<Expression<Func<Observation, bool>>>();

                var statId = int.Parse(_nanoqStationId);
                var t1 = new DateTime(1953, 1, 1, 0, 0, 0);
                var t2 = new DateTime(1953, 1, 1, 23, 59, 59);

                predicates.Add(o => o.StatId == statId);
                predicates.Add(o => o.ParamId == "temp_dry");
                predicates.Add(o => o.Time >= _t0);
                predicates.Add(o => o.Time <= _t1);

                // New way
                var observingFacility = unitOfWork.ObservingFacilities
                    .Find(_ => _.StatId == statId)
                    .SingleOrDefault();

                if (observingFacility == null)
                {
                    return;
                }

                observingFacility = unitOfWork.ObservingFacilities
                    .GetIncludingTimeSeries(observingFacility.Id);

                if (observingFacility.TimeSeries == null)
                {
                    return;
                }

                var timeSeries = observingFacility.TimeSeries
                    .Where(_ => _.ParamId == "temp_dry")
                    .SingleOrDefault();

                if (timeSeries == null)
                {
                    return;
                }

                timeSeries = unitOfWork.TimeSeries.GetIncludingObservations(
                    timeSeries.Id, _t0, _t1);

                if (timeSeries.Observations == null)
                {
                    return;
                }

                var observations = timeSeries.Observations
                    .Where(_ => _.Time >= _t0)
                    .Where(_ => _.Time <= _t1);

                var points = new List<PointD>();

                foreach (var observation in observations)
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
