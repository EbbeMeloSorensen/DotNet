using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Graph
{
    public class GraphViewModel : ViewModelBase
    {
        private string _greeting = "Greetings from GraphViewModel";

        public string Greeting
        {
            get { return _greeting; }
            set
            {
                _greeting = value;
                RaisePropertyChanged();
            }
        }
    }
}
