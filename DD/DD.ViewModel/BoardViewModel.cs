using Craft.Utils;
using Craft.ViewModels.Common;
using DD.Domain;
using DD.Application;
using System.Collections.Generic;
using System.Linq;

namespace DD.ViewModel
{
    /// <summary>
    /// Owns the view model collections for the obstacles and creatures on the board
    /// </summary>
    public class BoardViewModel : BoardViewModelBase
    {
        private List<PixelViewModel> _pixelViewModels;

        public List<PixelViewModel> PixelViewModels
        {
            get { return _pixelViewModels; }
            set
            {
                _pixelViewModels = value;
                RaisePropertyChanged();
            }
        }

        public BoardViewModel(
            IEngine engine, 
            double tileCenterSpacing, 
            double obstacleDiameter, 
            double creatureDiameter, 
            double weaponDiameter, 
            ObservableObject<Scene> selectedScene) : base(
                engine,
                tileCenterSpacing, 
                obstacleDiameter, 
                creatureDiameter, 
                weaponDiameter, 
                selectedScene)
        {
        }

        public override void LayoutBoard(
            Scene scene)
        {
            if (scene == null)
            {
                Rows = 0;
                Columns = 0;
                BoardWidth = 0;
                BoardHeight = 0;
                ImageWidth = 0;
                ImageHeight = 0;
                ScrollableOffset = new PointD(0, 0);
                ScrollOffset = new PointD(0, 0);

                PixelViewModels = new List<PixelViewModel>();
            }
            else
            {
                Rows = scene.Rows;
                Columns = scene.Columns;
                BoardWidth = Columns * TileCenterSpacing;
                BoardHeight = Rows * TileCenterSpacing;
                ImageWidth = BoardWidth;
                ImageHeight = BoardHeight;

                PixelViewModels = Enumerable.Range(0, Rows * Columns)
                    .Select(i => new PixelViewModel(i, new Pixel(200, 200, 200, 0)))
                    .ToList();
            }
        }

        public override void DetermineCanvasPosition(
            int positionX,
            int positionY,
            double diameter,
            out double left,
            out double top)
        {
            left = (positionX + 0.5) * TileCenterSpacing - diameter / 2;
            top = (positionY + 0.5) * TileCenterSpacing - diameter / 2;
        }

        public override void HighlightPlayerOptions(
            int squareIndexOfCurrentCreature,
            HashSet<int> squareIndexesCurrentCreatureCanMoveTo,
            HashSet<int> squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
            HashSet<int> squareIndexesCurrentCreatureCanAttackWithRangedWeapon)
        {
            PixelViewModels = Enumerable.Range(0, Rows * Columns)
                .Select(index => GeneratePixel(
                    index,
                    squareIndexOfCurrentCreature,
                    squareIndexesCurrentCreatureCanMoveTo,
                    squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                    squareIndexesCurrentCreatureCanAttackWithRangedWeapon))
                .ToList();
        }

        public override void ClearPlayerOptions()
        {
            PixelViewModels = Enumerable.Range(0, Rows * Columns)
                .Select(index => new PixelViewModel(index, new Pixel(200, 200, 200, 0)))
                .ToList();
        }
    }
}
