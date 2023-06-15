using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    // Todo: Erstat med en bounding box
    public delegate List<PointD> UpdatePointsCallback(
        double x0, 
        double y0);

    public class ScatterChartViewModel : MathematicalGeometryEditorViewModel
    {
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;

        private int _worldWindowUpdateCount;

        public int WorldWindowUpdateCount
        {
            get { return _worldWindowUpdateCount; }
            set
            {
                _worldWindowUpdateCount = value;
                RaisePropertyChanged();
            }
        }

        public UpdatePointsCallback UpdatePointsCallback { get; set; }

        //public ScatterChartViewModel(
        //    UpdatePointsCallback updatePointsCallback,
        //    double? initialMagnificationX,
        //    double? initialMagnificationY,
        //    Point? initialWorldWindowUpperLeft,
        //    Size? initialWorldWindowSize,
        //    int yAxisFactor) : base(initialMagnificationX,
        //                            initialMagnificationY,
        //                            initialWorldWindowUpperLeft,
        //                            initialWorldWindowSize,
        //                            yAxisFactor)
        //{
        //    UpdatePointsCallback = updatePointsCallback;

        //    PropertyChanged += ScatterChartViewModel_PropertyChanged;
        //}

        public ScatterChartViewModel(
            UpdatePointsCallback updatePointsCallback,
            int yAxisFactor,
            double scalingX,
            double scalingY) : base(yAxisFactor, scalingX, scalingY)
        {
            UpdatePointsCallback = updatePointsCallback;

            PropertyChanged += ScatterChartViewModel_PropertyChanged;
        }

        public ScatterChartViewModel(
            int yAxisFactor,
            double scalingX,
            double scalingY,
            Point worldWindowFocus) : base(yAxisFactor, scalingX, scalingY, worldWindowFocus)
        {
        }

        public ScatterChartViewModel(
            UpdatePointsCallback updatePointsCallback,
            int yAxisFactor,
            Point worldWindowFocus,
            Size worldWindowSize) : base(yAxisFactor, worldWindowFocus, worldWindowSize)
        {
            UpdatePointsCallback = updatePointsCallback;

            PropertyChanged += ScatterChartViewModel_PropertyChanged;
        }

        protected ScatterChartViewModel(
            UpdatePointsCallback updatePointsCallback,
            int yAxisFactor) : base(yAxisFactor)
        {
            UpdatePointsCallback = updatePointsCallback;

            PropertyChanged += ScatterChartViewModel_PropertyChanged;
        }

        private void ScatterChartViewModel_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "WorldWindowUpperLeft":
                case "WorldWindowSize":
                    UpdateCurve();
                    WorldWindowUpdateCount++;
                    break;
            }
        }

        private void UpdateCurve()
        {
            var x0 = Math.Floor(WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(WorldWindowUpperLeft.X + WorldWindowSize.Width);

            if (UpdatePointsCallback != null)
            {
                var points = UpdatePointsCallback(x0, x1);

                // Der er noget flicker. Man kunne måske forbedre det ved at tilføje polylines for de segmenter, der kommer ind
                // .. men det er lidt luksus, ok?
                PolylineViewModels.Clear();
                AddPolyline(points, _curveThickness, _curveBrush);
            }
        }
    }
}
