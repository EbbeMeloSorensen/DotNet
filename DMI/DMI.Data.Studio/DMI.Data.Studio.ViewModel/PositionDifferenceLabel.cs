using System.Windows.Media;
using Craft.ViewModels.Geometry2D.ScrollFree;

namespace DMI.Data.Studio.ViewModel;

public class PositionDifferenceLabel : LabelViewModel
{
    private Brush _backgroundBrush;
    private Brush _foregroundBrush;

    public Brush BackgroundBrush
    {
        get { return _backgroundBrush; }
        set
        {
            _backgroundBrush = value;
            RaisePropertyChanged();
        }
    }

    public Brush ForegroundBrush
    {
        get { return _foregroundBrush; }
        set
        {
            _foregroundBrush = value;
            RaisePropertyChanged();
        }
    }

    public PositionDifferenceLabel(
        Brush backgroundBrush,
        Brush foregroundBrush)
    {
        BackgroundBrush = backgroundBrush;
        ForegroundBrush = foregroundBrush;
    }
}
