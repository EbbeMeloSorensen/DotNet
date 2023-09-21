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

            MouseLeave += GeometryEditorView_MouseLeave;
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

            if (Keyboard.IsKeyDown(Key.LeftShift) || true) // Så længe det bare er en viewer, opererer vi med at man pr default panner, når man dragger
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

        private void UIElement_OnMouseLeftButtonUp(
            object sender, 
            MouseButtonEventArgs e)
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
                var xFactor = ViewModel.XAxisLocked ? 1.0 : 2.0;
                var yFactor = ViewModel.YAxisLocked ? 1.0 : 2.0;

                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width * xFactor,
                    ViewModel.Scaling.Height * yFactor);

                ViewModel.OnWorldWindowMajorUpdateOccured();
            }
            else
            {
                // Lets wait a bit with the create part and focus on the view part
                //ViewModel.PointViewModels.Add(new PointViewModel(ViewModel.MousePositionWorld));
            }
        }

        private void UIElement_OnMouseRightButtonDown(
            object sender, 
            MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            _mouseDownViewport = e.GetPosition((FrameworkElement)sender);
        }

        private void UIElement_OnMouseRightButtonUp(
            object sender, 
            MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }
            
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var xFactor = ViewModel.XAxisLocked ? 1.0 : 0.5;
                var yFactor = ViewModel.YAxisLocked ? 1.0 : 0.5;

                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width * xFactor,
                    ViewModel.Scaling.Height * yFactor);

                ViewModel.OnWorldWindowMajorUpdateOccured();
            }
        }

        private void UIElement_OnMouseMove(
            object sender, 
            System.Windows.Input.MouseEventArgs e)
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

                var x = ViewModel.XAxisLocked
                    ? ViewModel.WorldWindowUpperLeft.X
                    : _worldWindowUpperLeftInitial.X - mouseOffsetViewPort.X / ViewModel.Scaling.Width;

                var y = ViewModel.YAxisLocked
                    ? ViewModel.WorldWindowUpperLeft.Y
                    : _worldWindowUpperLeftInitial.Y - mouseOffsetViewPort.Y / ViewModel.Scaling.Height;

                ViewModel.WorldWindowUpperLeft = new Point(x, y);

                ViewModel.OnWorldWindowUpdateOccured();
            }
            else
            {
                ViewModel.OnMousePositionChanged(mouseViewPosition);
            }
        }

        private void UIElement_OnMouseWheel(
            object sender, 
            MouseWheelEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            var xFactor = ViewModel.XAxisLocked ? 1.0 : 1.2;
            var yFactor = ViewModel.YAxisLocked ? 1.0 : 1.2;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                yFactor = 1.0;
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                xFactor = 1.0;
            }

            if (e.Delta > 0)
            {
                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width * xFactor,
                    ViewModel.Scaling.Height * yFactor);
            }
            else
            {
                ViewModel.Scaling = new Size(
                    ViewModel.Scaling.Width / xFactor,
                    ViewModel.Scaling.Height / yFactor);
            }

            ViewModel.OnWorldWindowUpdateOccured();
            ViewModel.OnWorldWindowMajorUpdateOccured();
        }

        private void GeometryEditorView_MouseLeave(
            object sender,
            System.Windows.Input.MouseEventArgs e)
        {
            // Her har vi f.eks. mulighed for at skjule et koordinatsæt i brugergrænsefladen
        }

        private void UpdateModel(
            object sender, 
            EventArgs e)
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
