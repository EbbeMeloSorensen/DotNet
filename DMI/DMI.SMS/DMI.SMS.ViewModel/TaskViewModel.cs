using GalaSoft.MvvmLight;

namespace DMI.SMS.ViewModel
{
    public class TaskViewModel : ViewModelBase
    {
        private bool _busy;
        private double _progress;
        private string _currentActivity;

        public bool Busy
        {
            get => _busy;
            set
            {
                _busy = value;
                RaisePropertyChanged();
            }
        }

        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentActivity
        {
            get
            {
                return _currentActivity;
            }
            set
            {
                _currentActivity = value;
                RaisePropertyChanged();
            }
        }

        public TaskViewModel()
        {
            _progress = 0;
            _currentActivity = "";
        }
    }
}
