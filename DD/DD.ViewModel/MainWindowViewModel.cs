using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using DD.Domain;
using DD.Engine.Complex;
using DD.Application;

namespace DD.ViewModel
{
    public class MainWindowViewModel
    {
        private readonly Application.Application _application;

        public SceneCollectionViewModel SceneCollectionViewModel { get; }
        public BoardViewModelBase BoardViewModel { get; }
        public ActOutSceneViewModelBase ActOutSceneViewModel { get; }
        public LogViewModel LogViewModel { get; }

        public MainWindowViewModel(
            Application.Application application)
        {
            _application = application;

            LogViewModel = new LogViewModel();

            //_application.Logger = new ViewModelLogger(_application.Logger, LogViewModel);
            _application.Logger = null;
            _application.Logger?.WriteLine(LogMessageCategory.Debug, "Dungeons and Dragons - starting up");

            var selectedScene = new ObservableObject<Scene>();
            SceneCollectionViewModel = new SceneCollectionViewModel(selectedScene);

            var tileCenterSpacing = 80;
            var obstacleDiameter = 80;
            var creatureDiameter = 75;
            var projectileDiameter = 75;

            //var engine = new ComplexEngine(_application.Logger);
            var engine = new SimpleEngine(_application.Logger);

            engine.BoardTileMode = BoardTileMode.Hexagonal;
            //engine.BoardTileMode = BoardTileMode.Square;

            _application.Engine = engine;

            BoardViewModel = engine.BoardTileMode == BoardTileMode.Square
                ? new BoardViewModel(
                    engine: engine,
                    tileCenterSpacing,
                    obstacleDiameter,
                    creatureDiameter,
                    projectileDiameter,
                    selectedScene)
                : new BoardViewModelHex(
                    engine: engine,
                    tileCenterSpacing,
                    obstacleDiameter,
                    creatureDiameter,
                    projectileDiameter,
                    selectedScene);

            if (engine is ComplexEngine)
            {
                ActOutSceneViewModel = new ActOutSceneViewModelComplexEngine(
                    _application.Engine,
                    BoardViewModel,
                    selectedScene,
                    _application.Logger);
            }
            else
            {
                ActOutSceneViewModel = new ActOutSceneViewModelSimpleEngine(
                    _application.Engine,
                    BoardViewModel,
                    selectedScene,
                    _application.Logger);
            }

        }
    }
}
