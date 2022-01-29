using System;
using System.Collections.ObjectModel;
using System.Linq;
using DD.Application;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DD.ViewModel
{
    public class CreatureTypeCollectionViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;

        private RelayCommand<CreatureTypeViewModel> _selectionChangedCommand;
        private RelayCommand _populateListCommand;

        public ObservableCollection<CreatureTypeViewModel> CreatureTypeViewModels { get; set; }

        public RelayCommand<CreatureTypeViewModel> SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<CreatureTypeViewModel>(SelectionChanged)); }
        }

        public RelayCommand PopulateListCommand
        {
            get
            {
                return _populateListCommand ?? (_populateListCommand = new RelayCommand(
                    PopulateList));
            }
        }

        public CreatureTypeCollectionViewModel(
            IUIDataProvider dataProvider)
        {
            _dataProvider = dataProvider;

            CreatureTypeViewModels = new ObservableCollection<CreatureTypeViewModel>();

            dataProvider.CreatureTypeCreated += (s, e) =>
            {
                PopulateList();
            };
        }

        private void SelectionChanged(
            CreatureTypeViewModel creatureTypeViewModel)
        {
            //int a = 0;
        }

        private void PopulateList()
        {
            var creatureTypes = _dataProvider.GetAllCreatureTypes();

            CreatureTypeViewModels.Clear();

            creatureTypes.ToList().ForEach(
                ct => CreatureTypeViewModels.Add(new CreatureTypeViewModel(ct)));
        }
    }
}
