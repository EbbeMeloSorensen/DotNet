using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Application;

namespace DMI.StatDB.ViewModel
{
    public class StationListViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private IList<Station> _stations;
        private int _itemCount;
        private string _itemCountText;

        private ObservableCollection<StationViewModel> _stationViewModels;

        private RelayCommand _selectionChangedCommand;
        private RelayCommand<object> _findStationsCommand;

        public FindStationsViewModel FindStationsViewModel { get; private set; }

        public ObservableCollection<StationViewModel> StationViewModels
        {
            get { return _stationViewModels; }
            set
            {
                _stationViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObjectCollection<Station> SelectedStations { get; private set; }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                RaisePropertyChanged();

                UpdateItemCountText();
            }
        }

        public string ItemCountText
        {
            get { return _itemCountText; }
            set
            {
                _itemCountText = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand(SelectionChanged)); }
        }

        public RelayCommand<object> FindStationsCommand
        {
            get
            {
                return _findStationsCommand ?? (_findStationsCommand = new RelayCommand<object>(FindStations));
            }
        }

        public StationListViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;

            FindStationsViewModel = new FindStationsViewModel();
            SelectedStations = new ObjectCollection<Station>();
        }

        private void FindStations(
            object owner)
        {
            try
            {
                var limit = 100;

                //var stationsMatchingFilter = RetrieveStationsMatchingFilterFromRepository();
                //var count = stationsMatchingFilter.Count;

                var count = CountStationsMatchingFilterFromRepository();

                if (count == 0)
                {
                    var dialogViewModel = new MessageBoxDialogViewModel("No station matches the search criteria", false);
                    _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
                }

                if (count > limit)
                {
                    var dialogViewModel = new MessageBoxDialogViewModel(
                        $"{count} station rows match the search criteria.\nDo you want to retrieve them all from the repository?", true);

                    if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                var stationsMatchingFilter = RetrieveStationsMatchingFilterFromRepository();
                _stations = stationsMatchingFilter;
                ItemCount = count;

                UpdateStationViewModels();
            }
            catch (Exception e)
            {
                var dialogViewModel = new MessageBoxDialogViewModel("Query failed\nDid you provide valid credentials under File->Settings?", false);
                _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
            }
        }

        private int CountStationsMatchingFilterFromRepository()
        {
            if (FindStationsViewModel.FilterInPlace)
            {
                return _dataProvider.CountStations(FindStationsViewModel.FilterAsExpressionCollection());
            }
            else
            {
                return _dataProvider.CountAllStations();
            }
        }

        private IList<Station> RetrieveStationsMatchingFilterFromRepository()
        {
            IList<Station> result;

            if (FindStationsViewModel.FilterInPlace)
            {
                result = _dataProvider.FindStationsWithPositions(FindStationsViewModel.FilterAsExpressionCollection());
            }
            else
            {
                result = _dataProvider.GetAllStations();
            }

            return result;
        }

        private void UpdateStationViewModels()
        {
            // Selected stations should stay selected
            var idsOfSelectedStationInformations = new int[] { };

            if (StationViewModels != null)
            {
                idsOfSelectedStationInformations = StationViewModels
                    .Where(svm => svm.IsSelected)
                    .Select(svm => svm.Station.StatID)
                    .ToArray();
            }

            //UpdateSorting();

            StationViewModels = new ObservableCollection<StationViewModel>(_stations.Select(s => new StationViewModel
            {
                IsSelected = idsOfSelectedStationInformations.Contains(s.StatID),
                Station = s
            }));
        }

        private void UpdateItemCountText()
        {
            ItemCountText = ItemCount == 1
                ? "1 record"
                : $"{ItemCount} records";
        }

        private void SelectionChanged()
        {
            UpdateStationSelection();
        }

        private void UpdateStationSelection()
        {
            var temp = _stationViewModels
                .Where(s => s.IsSelected)
                .Select(s => s.Station)
                .ToList();

            SelectedStations.Objects = temp;
        }
    }
}
