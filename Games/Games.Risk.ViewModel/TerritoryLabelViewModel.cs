using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Games.Risk.ViewModel
{
    public class TerritoryLabelViewModel : ViewModelBase
    {
        private PointD _point;
        private string _text;

        public PointD Point
        {
            get => _point;
            set
            {
                _point = value;
                RaisePropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }
    }
}
