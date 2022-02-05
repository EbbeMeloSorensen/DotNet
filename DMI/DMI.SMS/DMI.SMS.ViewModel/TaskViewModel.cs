using GalaSoft.MvvmLight;

namespace DMI.SMS.ViewModel
{
    public class TaskViewModel : ViewModelBase
    {
        private double _progress;
        private string _currentActivity;

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
            Progress = 50;
            CurrentActivity = "Bamse";
        }
    }
}
