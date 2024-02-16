using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using DD.Application;
using DD.Domain;
using DD.Engine.Complex;

//using DD.Engine.Complex;

namespace DD.ViewModel
{
    public class MainWindowViewModel
    {
        private readonly Application.Application _application;
        private readonly IDialogService _applicationDialogService;

        private RelayCommand _windowLoadedCommand;
        private RelayCommand<object> _createCreatureTypeCommand;

        public SceneCollectionViewModel SceneCollectionViewModel { get; }
        public BoardViewModel BoardViewModel { get; }
        public ActOutSceneViewModelBase ActOutSceneViewModel { get; }
        public LogViewModel LogViewModel { get; }

        public RelayCommand WindowLoadedCommand
        {
            get { return _windowLoadedCommand ?? (_windowLoadedCommand = new RelayCommand(WindowLoaded)); }
        }

        public MainWindowViewModel(
            Application.Application application,
            IDialogService applicationDialogService)
        {
            _application = application;
            _applicationDialogService = applicationDialogService;

            LogViewModel = new LogViewModel();

            _application.Logger = new ViewModelLogger(_application.Logger, LogViewModel);
            //_application.Logger = null;
            _application.Logger?.WriteLine(LogMessageCategory.Debug, "Dungeons and Dragons - starting up");

            var selectedScene = new ObservableObject<Scene>();
            SceneCollectionViewModel = new SceneCollectionViewModel(selectedScene);

            var squareLength = 80;
            var obstacleDiameter = 80;
            var creatureDiameter = 75;
            var projectileDiameter = 75;

            var engine = new ComplexEngine(_application.Logger);
            //var engine = new SimpleEngine(_application.Logger);

            _application.Engine = engine;

            BoardViewModel = new BoardViewModel(
                engine: engine,
                squareLength,
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

        private void WindowLoaded()
        {
            //CreatureTypeCollectionViewModel.PopulateListCommand.Execute(null);
        }
    }
}
