using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Chronology;

namespace Craft.UIElements.Chronology
{
    /// <summary>
    /// Interaction logic for ChronologyView.xaml
    /// </summary>
    public partial class ChronologyView : UserControl
    {
        private PointD _mouseDownViewport;
        private PointD _initialScrollOffset;
        private bool _dragging;

        private ChronologyViewModel ViewModel
        {
            get { return DataContext as ChronologyViewModel; }
        }

        public ChronologyView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            // Is the scroll change caused by a change in the size of the embedded Canvas?
            if (System.Math.Abs(e.ExtentWidthChange) > 0.0000001 ||
                System.Math.Abs(e.ExtentHeightChange) > 0.0000001)
            {
                ViewModel.ScrollableOffset = new PointD(
                    ScrollViewer.ScrollableWidth,
                    ScrollViewer.ScrollableHeight);

                ViewModel.ScrollOffset = new PointD(0, 0);

                return;
            }

            // Otherwise we assume it is because the user interacted with a scrollbar 
            ViewModel.ScrollOffset = new PointD(
                e.HorizontalOffset,
                e.VerticalOffset);
        }

        private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            _mouseDownViewport = e.GetPosition(ScrollViewer).AsPointD();
            _initialScrollOffset = ViewModel.ScrollOffset;
            Canvas.CaptureMouse();
            _dragging = true;
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            _dragging = false;
            Canvas.ReleaseMouseCapture();
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            var mouseClientPosition = e.GetPosition(ScrollViewer).AsPointD();

            if (_dragging)
            {
                var offset = mouseClientPosition - _mouseDownViewport;

                ViewModel.ScrollOffset = new PointD(
                    System.Math.Min(System.Math.Max(0, _initialScrollOffset.X - offset.X), ViewModel.ScrollableOffset.X),
                    System.Math.Min(System.Math.Max(0, _initialScrollOffset.Y - offset.Y), ViewModel.ScrollableOffset.Y));
            }
            else
            {
                ViewModel.MouseWorldPosition = (ViewModel.ScrollOffset + mouseClientPosition) / ViewModel.Magnification;
            }
        }
    }
}
