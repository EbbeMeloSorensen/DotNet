using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Craft.ViewModels.Geometry2D;

namespace Craft.UIElements.Geometry2D
{
    /// <summary>
    /// Interaction logic for GeometryEditorView.xaml
    /// </summary>
    public partial class GeometryEditorView : UserControl
    {
        private Size _initialViewPortSize;
        private Point _mouseDownViewport;
        private Point _worldWindowUpperLeftInitial;
        private bool _dragging;

        private GeometryEditorViewModel ViewModel => DataContext as GeometryEditorViewModel;

        public GeometryEditorView()
        {
            InitializeComponent();

            CompositionTarget.Rendering += UpdateModel;
            DataContextChanged += GeometryEditorView_DataContextChanged;
        }

        private void GeometryEditorView_DataContextChanged(
            object sender, 
            DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.ViewPortSize = _initialViewPortSize;
        }

        private void FrameworkElement_OnSizeChanged(
            object sender, 
            SizeChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                // During startup, we have to set the size of the Viewport property in the ViewModel, but since OnSizeChanged may be called
                // before the DataContext is initialized, we store it in a member variable, and set it in the GeometryEditorView_DataContextChanged
                // handler, i.e. when the DataContext is initialized
                _initialViewPortSize = e.NewSize;
                return;
            }

            ViewModel.ViewPortSize = e.NewSize;
        }

        private void UIElement_OnMouseLeftButtonDown(
            object sender, 
            MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            _mouseDownViewport = e.GetPosition((FrameworkElement)sender);

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                _worldWindowUpperLeftInitial = ViewModel.WorldWindowUpperLeft;
                _dragging = true;
                Canvas.CaptureMouse();
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (_dragging)
            {
                _dragging = false;
                Canvas.ReleaseMouseCapture();
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ViewModel.Magnification *= 2;
            }
            else
            {
                // Lets wait a bit with the create part and focus on the view part
                //ViewModel.PointViewModels.Add(new PointViewModel(ViewModel.MousePositionWorld));
            }
        }

        private void UIElement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            _mouseDownViewport = e.GetPosition((FrameworkElement)sender);
        }

        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ViewModel.Magnification /= 2;
            }
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            var mouseViewPosition = e.GetPosition((FrameworkElement)sender);

            ViewModel.MousePositionViewport = mouseViewPosition;

            if (_dragging)
            {
                var mouseOffsetViewPort = new Point
                (
                    mouseViewPosition.X - _mouseDownViewport.X,
                    mouseViewPosition.Y - _mouseDownViewport.Y
                );

                ViewModel.WorldWindowUpperLeft = new Point(
                    _worldWindowUpperLeftInitial.X - mouseOffsetViewPort.X / ViewModel.Magnification,
                    _worldWindowUpperLeftInitial.Y - mouseOffsetViewPort.Y / ViewModel.Magnification);
            }
        }

        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (e.Delta > 0)
            {
                ViewModel.Magnification *= 1.2;
            }
            else
            {
                ViewModel.Magnification /= 1.2;
            }
        }

        private void UpdateModel(object sender, EventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            // Informer view modellen om at det er tid til opdatering
            ViewModel.UpdateModel();
        }
    }
}
