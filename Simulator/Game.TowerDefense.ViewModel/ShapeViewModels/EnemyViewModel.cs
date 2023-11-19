using Simulator.ViewModel.ShapeViewModels;

namespace Game.TowerDefense.ViewModel.ShapeViewModels;

public class EnemyViewModel : TaggedEllipseViewModel
{
    private string _imagePath;

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            RaisePropertyChanged();
        }
    }
}