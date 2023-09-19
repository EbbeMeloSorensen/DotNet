using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Craft.ViewModels.Geometry2D.ScrollFree;

namespace Craft.UIElements.Geometry2D.ScrollFree
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
            else
            {
                ViewModel.OnMouseClickOccured(_mouseDownViewport);
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
                ViewModel.OnWorldWindowMajorUpdateOccured();
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width * 2,
                    ViewModel.Scaling.Height * 2);

                ViewModel.OnWorldWindowMajorUpdateOccured();
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
                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width / 2,
                    ViewModel.Scaling.Height / 2);

                ViewModel.OnWorldWindowMajorUpdateOccured();
            }
        }

        private void UIElement_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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
                    _worldWindowUpperLeftInitial.X - mouseOffsetViewPort.X / ViewModel.Scaling.Width,
                    _worldWindowUpperLeftInitial.Y - mouseOffsetViewPort.Y / ViewModel.Scaling.Height);

                ViewModel.OnWorldWindowUpdateOccured();
            }
            else
            {
                ViewModel.OnMousePositionChanged(mouseViewPosition);
            }
        }

        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            var x_factor = 1.2;
            var y_factor = 1.2;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                y_factor = 1.0;
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                x_factor = 1.0;
            }

            if (e.Delta > 0)
            {
                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width * x_factor,
                    ViewModel.Scaling.Height * y_factor);
            }
            else
            {
                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width / x_factor,
                    ViewModel.Scaling.Height / y_factor);
            }

            ViewModel.OnWorldWindowUpdateOccured();
            ViewModel.OnWorldWindowMajorUpdateOccured();
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
