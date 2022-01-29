using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using DD.Domain;
using DD.Application;

namespace DD.ViewModel
{
    public class SceneCollectionViewModel : ViewModelBase
    {
        private ObservableObject<Scene> _selectedScene;
        private RelayCommand<SceneViewModel> _selectionChangedCommand;

        public ObservableCollection<SceneViewModel> SceneViewModels { get; set; }

        public RelayCommand<SceneViewModel> SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<SceneViewModel>(SelectionChanged)); }
        }

        public SceneCollectionViewModel(
            ObservableObject<Scene> selectedScene)
        {
            _selectedScene = selectedScene;

            var scenes = new List<Scene>
            {
                SceneGenerator.GenerateScene(1),
                SceneGenerator.GenerateScene(2),
                SceneGenerator.GenerateScene(3),
                SceneGenerator.GenerateScene(4),
                SceneGenerator.GenerateScene(5),
                SceneGenerator.GenerateScene(6),
                SceneGenerator.GenerateScene(7),
                SceneGenerator.GenerateScene(8),
                SceneGenerator.GenerateScene(9),
                SceneGenerator.GenerateScene(10),
                SceneGenerator.GenerateScene(11),
                SceneGenerator.GenerateScene(12),
                SceneGenerator.GenerateScene(13),
                SceneGenerator.GenerateScene(14),
                SceneGenerator.GenerateScene(15),
                SceneGenerator.GenerateScene(16),
            };

            SceneViewModels = new ObservableCollection<SceneViewModel>(scenes.Select(s => new SceneViewModel(s)));
        }

        private void SelectionChanged(SceneViewModel sceneViewModel)
        {
            _selectedScene.Object = sceneViewModel?.Object;
        }
    }
}
