using Craft.Utils;
using DD.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Craft.UI.Utils;

namespace DD.UI.WPF
{
    /// <summary>
    /// Interaction logic for BoardViewHexagons.xaml
    /// </summary>
    public partial class BoardViewHexagons : UserControl
    {
        private BoardViewModelHex ViewModel
        {
            get { return DataContext as BoardViewModelHex; }
        }

        public BoardViewHexagons()
        {
            InitializeComponent();
        }

        private void ScrollViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ViewModel == null) return;

            // Is the scroll change caused by a change in the size of the embedded Canvas?
            if (Math.Abs(e.ExtentWidthChange) > 0.0000001 ||
                Math.Abs(e.ExtentHeightChange) > 0.0000001)
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

        private void MoveCreatureStoryboard_Completed(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            CurrentCreatureGrid.RenderTransform = new TranslateTransform(0, 0);

            ViewModel.CompleteMoveCreatureAnimation();
        }

        private void AttackStoryboard_Completed(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            WeaponGrid.RenderTransform = new TranslateTransform(0, 0);

            ViewModel.CompleteAttackAnimation();
        }
    }
}
