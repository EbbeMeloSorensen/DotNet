using System;
using System.Collections.Generic;
using System.Linq;
using Craft.Utils;
using Craft.ViewModels.Common;
using DD.Application;
using DD.Domain;

namespace DD.ViewModel;

public class BoardViewModelHex : BoardViewModelBase
{
    private List<PixelViewModel> _pixelViewModels1;
    private List<PixelViewModel> _pixelViewModels2;

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

    public BoardViewModelHex(
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

    public override void LayoutBoard(
        Scene scene)
    {
        if (scene == null)
        {
            Rows = 0;
            Columns = 0;
            ImageWidth = 0;
            ImageHeight = 0;
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            PixelViewModels1 = new List<PixelViewModel>();
        }
        else
        {
            Rows = scene.Rows / 2;
            Columns = scene.Columns;
            ImageWidth = Columns * 36;
            ImageHeight = Rows * 2 * 36 * 0.866025404;

            var range1 = Enumerable.Range(0, Rows)
                .Select(i => Enumerable.Range(i * 2 * Columns, Columns))
                .SelectMany(_ => _);

            var range2 = Enumerable.Range(0, Rows)
                .Select(i => Enumerable.Range(i * 2 * Columns + Columns, Columns))
                .SelectMany(_ => _);

            PixelViewModels1 = range1
                .Select(i => new PixelViewModel(i, new Pixel(50, 50, 50, 255)))
                .ToList();

            PixelViewModels2 = range2
                .Select(i => new PixelViewModel(i, new Pixel(50, 50, 50, 255)))
                .ToList();
        }
    }

    public override void HighlightPlayerOptions(
        int squareIndexOfCurrentCreature,
        HashSet<int> squareIndexesCurrentCreatureCanMoveTo,
        HashSet<int> squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
        HashSet<int> squareIndexesCurrentCreatureCanAttackWithRangedWeapon)
    {
        throw new NotImplementedException();
    }

    public override void ClearPlayerOptions()
    {
        throw new NotImplementedException();
    }
}