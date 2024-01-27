using Craft.ViewModels.Common;
using System.Collections.Generic;
using System.Linq;
using Craft.Utils;
using GalaSoft.MvvmLight;

namespace Craft.Algorithms.GuiTest.Tab9
{
    // Dijkstra Shortest Path in a hex mesh
    public class Tab9ViewModel : ViewModelBase
    {
        private HashSet<int> _pixelIndexes;
        private List<PixelViewModel> _pixelViewModels1;
        private List<PixelViewModel> _pixelViewModels2;

        public int Rows
        {
            get { return 5; }
        }

        public int Cols
        {
            get { return 16; }
        }

        public List<PixelViewModel> PixelViewModels1
        {
            get { return _pixelViewModels1; }
            set
            {
                _pixelViewModels1 = value;
                RaisePropertyChanged();
            }
        }

        public List<PixelViewModel> PixelViewModels2
        {
            get { return _pixelViewModels2; }
            set
            {
                _pixelViewModels2 = value;
                RaisePropertyChanged();
            }
        }

        public Tab9ViewModel()
        {
            _pixelIndexes = new HashSet<int>();

            InitializePixels();
        }

        private void PixelLeftClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pixelIndexes.Add(e.ElementId);

            InitializePixels();
        }

        private void PixelRightClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            if (_pixelIndexes.Contains(e.ElementId))
            {
                _pixelIndexes.Remove(e.ElementId);
            }

            InitializePixels();
        }

        private void InitializePixels()
        {
            var range1 = Enumerable.Range(0, Rows)
                .Select(i => Enumerable.Range(i * 2 * Cols, Cols))
                .SelectMany(_ => _);

            var range2 = Enumerable.Range(0, Rows)
                .Select(i => Enumerable.Range(i * 2 * Cols + Cols, Cols))
                .SelectMany(_ => _);

            PixelViewModels1 = range1
                .Select(i => new PixelViewModel(i, _pixelIndexes.Contains(i)
                    ? new Pixel(255, 255, 255, 255, $"{i}")
                    : new Pixel(50, 50, 50, 255, $"{i}")))
                .ToList();

            PixelViewModels2 = range2
                .Select(i => new PixelViewModel(i, _pixelIndexes.Contains(i)
                    ? new Pixel(255, 255, 255, 255, $"{i}")
                    : new Pixel(50, 50, 50, 255, $"{i}")))
                .ToList();

            PixelViewModels1.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });

            PixelViewModels2.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }
    }
}
