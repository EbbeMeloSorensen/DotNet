using System;
using System.Collections.Generic;
using System.Windows;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using DD.Application;
using DD.Domain;
using GalaSoft.MvvmLight.Command;

namespace DD.ViewModel
{
    public class MainWindowViewModel
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private ILogger _logger;

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
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService,
            ILogger logger)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;

            var selectedScene = new ObservableObject<Scene>();

            LogViewModel = new LogViewModel();

            //_logger = logger;
            _logger = new ViewModelLogger(logger, LogViewModel);
            //_logger = new IdleLogger();
            _logger.WriteLine(LogMessageCategory.Debug, "Dungeons and Dragons - starting up");

            CreatureTypeCollectionViewModel = new CreatureTypeCollectionViewModel(dataProvider);
            SceneCollectionViewModel = new SceneCollectionViewModel(selectedScene);

            var squareLength = 80;
            var obstacleDiameter = 80;
            var creatureDiameter = 75;
            var projectileDiameter = 75;
            var squareIndexForCurrentCreature = new ObservableObject<int?>();
            var squareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            var squareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            var squareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            BoardViewModel = new BoardViewModel(
                squareLength,
                obstacleDiameter,
                creatureDiameter,
                projectileDiameter,
                selectedScene, 
                squareIndexForCurrentCreature, 
                squareIndexesCurrentCreatureCanMoveTo,
                squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                squareIndexesCurrentCreatureCanAttackWithRangedWeapon);

            SceneEditorViewModel = new SceneEditorViewModel(
                BoardViewModel);

            ActOutSceneViewModel = new ActOutSceneViewModel(
                BoardViewModel, 
                selectedScene, 
                squareIndexForCurrentCreature,
                squareIndexesCurrentCreatureCanMoveTo,
                squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                squareIndexesCurrentCreatureCanAttackWithRangedWeapon,
                _logger);
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

            _dataProvider.CreateCreatureType(new CreatureType(dialogViewModel.Name, 8, 5, 15, 0, 8, null));
        }

        private bool CanCreateCreatureType(object owner)
        {
            return true;
        }
    }
}
