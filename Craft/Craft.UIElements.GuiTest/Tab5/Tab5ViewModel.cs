using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab5
{
    public class Tab5ViewModel : ViewModelBase
    {
        private string _greeting = "Greetings from Tab5ViewModel";

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
