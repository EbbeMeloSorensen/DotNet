using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Math;

namespace Craft.Algorithms.GuiTest.Common
{
    public class Point2DViewModel : ViewModelBase
    {
        private int _index;
        private Point2D _point;
        private double _left;
        private double _top;
        private double _diameter;
        private double _radius;
        private RelayCommand _clickedCommand;

        public Point2D Point
        {
            get { return _point; }
            set
            {
                _point = value;
                UpdateTopLeft();
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ClickedCommand
        {
            get
            {
                return _clickedCommand ?? (_clickedCommand = new RelayCommand(Clicked));
            }
        }

        public event EventHandler<ElementClickedEventArgs> ElementClicked;

        public Point2DViewModel(
            Point2D point,
            int index,
            double diameter)
        {
            Point = point;
            _index = index;
            _diameter = diameter;
            _radius = _diameter / 2;

            UpdateTopLeft();
        }

        private void Clicked()
        {
            OnElementClicked();
        }

        private void OnElementClicked()
        {
            var handler = ElementClicked;

            if (handler != null)
            {
                handler(this, new ElementClickedEventArgs(_index));
            }
        }

        private void UpdateTopLeft()
        {
            Left = Point.X - _radius;
            Top = Point.Y - _radius;
        }
    }
}
