using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
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

        public ScatterChartViewModel(
            double initialMagnificationX = 1,
            double initialMagnificationY = 1,
            double initialWorldWindowUpperLeftX = 0,
            double initialWorldWindowUpperLeftY = 0) : base(initialMagnificationX,
                                                        initialMagnificationY,
                                                        initialWorldWindowUpperLeftX,
                                                        initialWorldWindowUpperLeftY)
        {
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
            // Todo: Find ud af hvilke X-værdier vi skal bruge

            var x0 = Math.Floor(WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(WorldWindowUpperLeft.X + WorldWindowSize.Width);

            var points = new List<PointD>();

            for (var x = x0; x <= x1; x += 0.1)
            {
                points.Add(new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) /4 - 3 * x / 2 - 2));
                //points.Add(new PointD(x, Math.Sin(x)));
            }

            // Der er noget flicker. Man kunne måske forbedre det ved at tilføje polylines for de segmenter, der kommer ind
            // .. men det er lidt luksus, ok?
            PolylineViewModels.Clear();
            AddPolyline(points, _curveThickness, _curveBrush);
        }
    }
}
