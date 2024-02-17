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
            double squareLength, 
            double obstacleDiameter, 
            double creatureDiameter, 
            double weaponDiameter, 
            ObservableObject<Scene> selectedScene) : base(
                engine, 
                squareLength, 
                obstacleDiameter, 
                creatureDiameter, 
                weaponDiameter, 
                selectedScene)
        {
        }

        public override void InitializePixelViewModels()
        {
            PixelViewModels = Enumerable.Range(0, Rows * Columns)
                .Select(i => new PixelViewModel(i, new Pixel(200, 200, 200, 0)))
                .ToList();
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
