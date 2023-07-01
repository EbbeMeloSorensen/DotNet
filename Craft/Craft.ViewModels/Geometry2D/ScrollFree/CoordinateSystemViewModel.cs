using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class CoordinateSystemViewModel : ViewModelBase
    {
        protected double _marginX;
        protected double _marginY;
        protected double _Y2;
        protected bool _includeGrid = true;
        protected Brush _gridBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.25 };
        protected Brush _curveBrush = new SolidColorBrush(Colors.Black);
        protected double _curveThickness = 0.05;

        public double MarginX
        {
            get
            {
                return _marginX;
            }
        }

        public double Y2 
        { 
            get
            {
                return _Y2;
            }
            set
            {
                _Y2 = value;
                RaisePropertyChanged();
            }
        }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public CoordinateSystemViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            double marginX,
            double marginY)
        {
            _marginX = marginX;
            _marginY = marginY;

            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1, worldWindowFocus, worldWindowSize);

            GeometryEditorViewModel.PropertyChanged += GeometryEditorViewModel_PropertyChanged;

            GeometryEditorViewModel.WorldWindowMajorUpdateOccured += 
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;

            GeometryEditorViewModel.WorldWindowUpdateOccured += 
                GeometryEditorViewModel_WorldWindowUpdateOccured;
        }

        private void GeometryEditorViewModel_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ViewPortSize":
                {
                    Y2 = GeometryEditorViewModel.ViewPortSize.Height - _marginY;
                    break;
                }
            }
        }

        private void GeometryEditorViewModel_WorldWindowUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            GeometryEditorViewModel.ClearLabels();
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            UpdateCoordinateSystemForGeometryEditorViewModel(
                e.WorldWindowUpperLeft.X,
                e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width,
                -e.WorldWindowUpperLeft.Y - e.WorldWindowSize.Height,
                -e.WorldWindowUpperLeft.Y);
        }

        protected virtual void UpdateCoordinateSystemForGeometryEditorViewModel(
            double x0,
            double x1,
            double y0,
            double y1)
        {
            // We want margins and thickness to be independent on scaling
            var dx = _marginX / GeometryEditorViewModel.Scaling.Width;
            var dy = _marginY / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            GeometryEditorViewModel.ClearLines();
            GeometryEditorViewModel.ClearLabels();

            DrawHorizontalGridLines(x0, y0, x1, y1, dx, dy, thickness);
            DrawVerticalGridLines(x0, y0, x1, y1, dx, dy, thickness);
        }

        protected void DrawHorizontalGridLines(
            double x0,
            double y0,
            double x1,
            double y1,
            double dx,
            double dy,
            double thickness)
        {
            // 1: Find ud af spacing af ticks for y-aksen
            var spacingY = 1.0;

            // Find ud af første y-værdi
            var y = Math.Floor(y0 / spacingY) * spacingY;

            while (y < y1)
            {
                if (y > y0 + dy)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x0 + dx, y),
                            new PointD(x1, y),
                            thickness,
                            _gridBrush);
                    }

                    var text = y.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0);
                }

                y += spacingY;
            }
        }

        protected virtual void DrawVerticalGridLines(
            double x0,
            double y0,
            double x1,
            double y1,
            double dx,
            double dy,
            double thickness)
        {
            // 1: Find ud af spacing af ticks for x-aksen
            // det må gerne afhænge af, hvor meget plads, der er, dvs hvad scaling er
            var spacingX = 1.0;
            var labelWidth = spacingX * GeometryEditorViewModel.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af første x-værdi
            var x = Math.Floor(x0 / spacingX) * spacingX;

            while (x < x1)
            {
                if (x > x0 + dx)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x, y0 + dy),
                            new PointD(x, y1),
                            thickness,
                            _gridBrush);
                    }

                    var text = x.ToString(CultureInfo.InvariantCulture);

                    // Place label at ticks
                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);
                }

                x += spacingX;
            }
        }
    }
}
