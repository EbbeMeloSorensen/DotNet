using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DD.ViewModel
{
    public class SceneEditorViewModel : ViewModelBase
    {
        private BoardViewModel _boardViewModel;
        private RelayCommand _clearSceneCommand;

        public RelayCommand ClearSceneCommand
        {
            get { return _clearSceneCommand ?? (_clearSceneCommand = new RelayCommand(ClearScene)); }
        }

        public SceneEditorViewModel(
            BoardViewModel boardViewModel)
        {
            _boardViewModel = boardViewModel;
        }

        private void ClearScene()
        {

        }
    }
}
