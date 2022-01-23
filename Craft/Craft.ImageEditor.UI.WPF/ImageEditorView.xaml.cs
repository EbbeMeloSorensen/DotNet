using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Craft.ImageEditor.ViewModel;
using Craft.UI.Utils;
using Craft.Utils;

namespace Craft.ImageEditor.UI.WPF
{
    /// <summary>
    /// Interaction logic for ImageEditorView.xaml
    /// </summary>
    public partial class ImageEditorView : UserControl
    {
        private PointD _mouseDownViewport;
        private PointD _initialScrollOffset;
        private bool _dragging;

        private ImageEditorViewModel ViewModel
        {
            get { return DataContext as ImageEditorViewModel; }
        }

        public ImageEditorView()
        {
            InitializeComponent();
        }
    }
}
