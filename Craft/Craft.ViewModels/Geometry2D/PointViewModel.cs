﻿using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D
{
    public class PointViewModel : ViewModelBase
    {
        private PointD _point;
        private double _diameter;
        private Brush _brush;

        public PointD Point
        {
            get => _point;
            set
            {
                _point = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter
        {
            get => _diameter;
            set
            {
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

        public PointViewModel(
            PointD point,
            double diameter,
            Brush brush)
        {
            Point = point;
            Diameter = diameter;
            Brush = brush;
        }
    }
}
