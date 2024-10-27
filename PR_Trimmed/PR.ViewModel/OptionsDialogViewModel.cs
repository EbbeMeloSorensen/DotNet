using System;
using Craft.ViewModels.Dialogs;

namespace PR.ViewModel
{
    public class OptionsDialogViewModel : DialogViewModelBase
    {
        private DateTime? _databaseTime;

        public DateTime? DatabaseTime
        {
            get { return _databaseTime; }
            set
            {
                _databaseTime = value;
                RaisePropertyChanged();
            }
        }

        public OptionsDialogViewModel(
            DateTime? databaseTime)
        {
            DatabaseTime = databaseTime;
        }
    }
}
