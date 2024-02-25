using GalaSoft.MvvmLight;
using DD.Domain;

namespace DD.ViewModel;

public class TeamStatsListItemViewModel : ViewModelBase
{
    private int _count;

    public CreatureType CreatureType { get; set; }

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            RaisePropertyChanged();
        }
    }
}