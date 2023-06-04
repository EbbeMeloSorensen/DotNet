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

        public ScatterChartViewModel(
            UpdatePointsCallback updatePointsCallback,
            double initialMagnificationX = 1,
            double initialMagnificationY = 1,
            double initialWorldWindowUpperLeftX = 0,
            double initialWorldWindowUpperLeftY = 0) : base(initialMagnificationX,
                                                        initialMagnificationY,
                                                        initialWorldWindowUpperLeftX,
                                                        initialWorldWindowUpperLeftY)
        {
            UpdatePointsCallback = updatePointsCallback;

            _worldWindowLowerLeft = new Point(
                initialWorldWindowUpperLeftX,
                initialWorldWindowUpperLeftY);

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
