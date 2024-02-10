using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using DD.Application;
using DD.Domain;
using DD.Engine.Complex;

namespace DD.ViewModel
{
    public class MainWindowViewModel
    {
        private readonly Application.Application _application;
        private readonly IDialogService _applicationDialogService;

        private RelayCommand _windowLoadedCommand;
        private RelayCommand<object> _createCreatureTypeCommand;

        public CreatureTypeCollectionViewModel CreatureTypeCollectionViewModel { get; }
        public SceneCollectionViewModel SceneCollectionViewModel { get; }
        public BoardViewModel BoardViewModel { get; }
        public ActOutSceneViewModel ActOutSceneViewModel { get; }
        public SceneEditorViewModel SceneEditorViewModel { get; }
        public LogViewModel LogViewModel { get; }

        public RelayCommand WindowLoadedCommand
        {
            get { return _windowLoadedCommand ?? (_windowLoadedCommand = new RelayCommand(WindowLoaded)); }
        }

        public RelayCommand<object> CreateCreatureTypeCommand
        {
            get { return _createCreatureTypeCommand ?? (_createCreatureTypeCommand = new RelayCommand<object>(CreateCreatureType, CanCreateCreatureType)); }
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
            CreatureTypeCollectionViewModel = new CreatureTypeCollectionViewModel(_application.UIDataProvider);

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

            SceneEditorViewModel = new SceneEditorViewModel(
                BoardViewModel);

            ActOutSceneViewModel = new ActOutSceneViewModel(
                _application.Engine,
                BoardViewModel, 
                selectedScene,
                _application.Logger);
        }

        private void WindowLoaded()
        {
            CreatureTypeCollectionViewModel.PopulateListCommand.Execute(null);
        }

        private void CreateCreatureType(
            object owner)
        {
            var dialogViewModel = new CreateCreatureTypeDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            _application.UIDataProvider.CreateCreatureType(
                new CreatureType(dialogViewModel.Name, 8, 5, 15, 0, 8, null));
        }

        private bool CanCreateCreatureType(object owner)
        {
            return true;
        }
    }
}
