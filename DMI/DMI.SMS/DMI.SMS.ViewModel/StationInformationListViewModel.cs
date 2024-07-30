using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using DMI.SMS.Application;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.ViewModel
{
    public enum Sorting
    {
        Smart,
        StationId,
        StationName
    }

    public class StationInformationListViewModel : ViewModelBase
    {
        // Todo: Smid den her over i Shared
        public readonly string[] HistoricallyRelevantFields = new string[8] 
        {
            "Hha",
            "Hhp",
            "StationName",
            "StationIDDMI",
            "Stationtype",
            "StationOwner",
            "Wgs_lat",
            "Wgs_long"
        };

        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private readonly ObservableObject<bool> _classifyRecordsWithCondition;
        private IList<StationInformation> _stationInformations;
        private Sorting _sorting;
        private int _itemCount;
        private string _itemCountText;

        private ObservableCollection<StationInformationViewModel> _stationInformationViewModels;

        private Dictionary<RowCondition, Brush> _conditionToBrushMap = new Dictionary<RowCondition, Brush>
        {
            { RowCondition.Current, new SolidColorBrush(Colors.DarkGreen) },
            { RowCondition.OutDated, new SolidColorBrush(Colors.PaleGoldenrod) },
            { RowCondition.Deleted, new SolidColorBrush(Colors.DarkRed) }
        };

        private Brush _backgroundBrush1 = new SolidColorBrush(Colors.White);
        private Brush _backgroundBrush2 = new SolidColorBrush(Colors.LightGray);

        private RelayCommand<object> _selectionChangedCommand;
        private RelayCommand<object> _findStationInformationsCommand;
        private RelayCommand _clearFiltersCommand;

        public FindStationInformationsViewModel FindStationInformationsViewModel { get; private set; }

        public ObservableCollection<StationInformationViewModel> StationInformationViewModels
        {
            get { return _stationInformationViewModels; }
            set
            {
                _stationInformationViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObjectCollection<StationInformation> StationInformations { get; private set; }
        public ObjectCollection<StationInformation> SelectedStationInformations { get; private set; }

        public Sorting Sorting
        {
            get { return _sorting; }
            set
            {
                _sorting = value;
                RaisePropertyChanged();
                UpdateStationInformationViewModels();
            }
        }

        public ObservableObject<Dictionary<int, RowCharacteristics>> RowCharacteristicsMap { get; set; }

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

        public RelayCommand<object> SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<object>(SelectionChanged)); }
        }

        public RelayCommand<object> FindStationInformationsCommand
        {
            get
            {
                return _findStationInformationsCommand ?? (_findStationInformationsCommand = new RelayCommand<object>(FindStationInformations));
            }
        }

        public RelayCommand ClearFiltersCommand
        {
            get
            {
                return _clearFiltersCommand ?? (_clearFiltersCommand = new RelayCommand(ClearFilters));
            }
        }

        public void Refresh()
        {
            FetchStationInformationsFromRepository();
        }

        public StationInformationListViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService,
            ObservableObject<bool> classifyRecordsWithCondition)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;
            _classifyRecordsWithCondition = classifyRecordsWithCondition;

            _sorting = Sorting.Smart;

            FindStationInformationsViewModel = new FindStationInformationsViewModel();

            StationInformations = new ObjectCollection<StationInformation>();
            SelectedStationInformations = new ObjectCollection<StationInformation>();
            RowCharacteristicsMap = new ObservableObject<Dictionary<int, RowCharacteristics>>();
        }

        private int CountStationInformationsMatchingFilterFromRepository(
            bool avoidTwoCallsToTheDatabaseInCaseInMemoryConditionFilteringIsRequired = false)
        {
            if (FindStationInformationsViewModel.FilterInPlace)
            {
                if (FindStationInformationsViewModel.ConditionFilteringInMemoryRequired ||
                    _classifyRecordsWithCondition.Object == true)
                {
                    // We cannot make the database count,
                    // so we have to pull the entire dataset into memory and do the counting there
                    var records = _dataProvider.GetAllStationInformations();
                    var localRowCharacteristicsMap = records.GenerateRowCharacteristicsMap();

                    // Nu skal du så til gengæld bruge det filter, der jo altså er.
                    // Jeg ville gerne gøre det i memory, men det går galt, når man f.eks. bruger et filter, hvor der er et antal tilladte
                    // stationstyper, og hvor man så har en "contains"-instruktion i en expression
                    // (Undersøg lige, om det her overhovedet er nødvendigt.. I praksis gælder der jo altså altid, at
                    // parameteren er false..)
                    if (avoidTwoCallsToTheDatabaseInCaseInMemoryConditionFilteringIsRequired)
                    {
                        var filterExpressions = FindStationInformationsViewModel.FilterAsExpressionCollection().ToList();
                        filterExpressions.ForEach(fe =>
                        {
                            var deleg = fe.Compile();
                            records = records.Where(deleg).ToList();
                        });
                    }
                    else
                    {
                        records = _dataProvider.FindStationInformations(FindStationInformationsViewModel.FilterAsExpressionCollection());
                    }

                    records = records
                        .Where(s => FindStationInformationsViewModel.InMemoryConditionFilter.Contains(localRowCharacteristicsMap[s.GdbArchiveOid].RowCondition))
                        .ToList();

                    return records.Count;
                }
                else
                {
                    return _dataProvider.CountStationInformations(FindStationInformationsViewModel.FilterAsExpressionCollection());
                }
            }
            else
            {
                return _dataProvider.CountAllStationInformations();
            }
        }

        private IList<StationInformation> RetrieveStationInformationsMatchingFilterFromRepository(
            bool avoidTwoCallsToTheDatabaseInCaseInMemoryConditionFilteringIsRequired = false)
        {
            IList<StationInformation> result = null;
            Dictionary<int, RowCharacteristics> localRowCharacteristicsMap;

            if (FindStationInformationsViewModel.FilterInPlace)
            {
                if (FindStationInformationsViewModel.ConditionFilteringInMemoryRequired ||
                    _classifyRecordsWithCondition.Object)
                {
                    result = _dataProvider.GetAllStationInformations();
                    localRowCharacteristicsMap = result.GenerateRowCharacteristicsMap();

                    // Nu skal du så til gengæld bruge det filter, der jo altså er
                    if (avoidTwoCallsToTheDatabaseInCaseInMemoryConditionFilteringIsRequired)
                    {
                        var filterExpressions = FindStationInformationsViewModel.FilterAsExpressionCollection().ToList();
                        filterExpressions.ForEach(fe =>
                        {
                            result = result.Where(fe.Compile()).ToList();
                        });
                    }
                    else
                    {
                        result = _dataProvider.FindStationInformations(FindStationInformationsViewModel.FilterAsExpressionCollection());
                    }

                    // Nu foretager vi så den filtrering, som skal ske i memory på basis af de row conditions, vi har i characteristics mappet
                    result = result
                        .Where(s => FindStationInformationsViewModel.InMemoryConditionFilter.Contains(localRowCharacteristicsMap[s.GdbArchiveOid].RowCondition))
                        .ToList();
                }
                else
                {
                    result = _dataProvider.FindStationInformations(FindStationInformationsViewModel.FilterAsExpressionCollection());
                    localRowCharacteristicsMap = result.GenerateRowCharacteristicsMap();
                }
            }
            else
            {
                result = _dataProvider.GetAllStationInformations();
                localRowCharacteristicsMap = result.GenerateRowCharacteristicsMap();
            }

            RowCharacteristicsMap.Object = localRowCharacteristicsMap;

            return result;
        }

        private void UpdateSorting()
        {
            switch (Sorting)
            {
                case Sorting.Smart:
                    _stationInformations = _stationInformations.Sort();
                    break;
                case Sorting.StationId:
                    _stationInformations = _stationInformations
                        .OrderBy(s => s.StationIDDMI)
                        .ThenBy(s => s.ObjectId)
                        .ThenBy(s => s.DateFrom)
                        .ThenBy(s => s.GdbFromDate)
                        .ToList();
                    break;
                case Sorting.StationName:
                    _stationInformations = _stationInformations
                        .OrderBy(s => s.StationName)
                        .ThenBy(s => s.ObjectId)
                        .ThenBy(s => s.DateFrom)
                        .ThenBy(s => s.GdbFromDate)
                        .ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateStationInformationViewModels()
        {
            UpdateSorting();

            var businessRulesForIndividualRecords = new HashSet<BusinessRule>
            {
                BusinessRule.ACurrentRowMustHaveAStationName,
                BusinessRule.ACurrentRowMustNotHaveAStationNameWrittenInUpperCase,
                BusinessRule.ACurrentRowMustHaveAStationType,
                BusinessRule.ACurrentRowMustHaveACountry,
                BusinessRule.ACurrentRowMustHaveAStationOwner,
                BusinessRule.ACurrentRowMustHaveAStatus,
                BusinessRule.ACurrentRowWithStatusInactiveMustHaveADateTo
            };

            var businessRulesForRecordGroupsWithSameObjectId = new HashSet<BusinessRule>
            {
                BusinessRule.ObjectWasSubjectedToChangeOfNameSinceCreation,
                BusinessRule.ObjectWasSubjectedToChangeOfLocationSinceCreation,
            };

            var businessRulesForCurrentRecordGroupsWithSameStationId = new HashSet<BusinessRule>
            {
                BusinessRule.OverlappingCurrentRecordsWithSameStationIdExists
            };

            var stationInformationViewModels = _stationInformations.Select(s => 
            {
                var warning1 = 
                    RowCharacteristicsMap.Object.ContainsKey(s.GdbArchiveOid) && 
                    RowCharacteristicsMap.Object[s.GdbArchiveOid].ViolatedBusinessRules.Intersect(businessRulesForIndividualRecords).Any();

                return new StationInformationViewModel
                {
                    StationInformation = s,
                    Brush = _conditionToBrushMap[RowCharacteristicsMap.Object[s.GdbArchiveOid].RowCondition],
                    Warning1 = warning1,
                    Warning2 = RowCharacteristicsMap.Object.ContainsKey(s.GdbArchiveOid) && RowCharacteristicsMap.Object[s.GdbArchiveOid].ViolatedBusinessRules.Intersect(businessRulesForRecordGroupsWithSameObjectId).Any(),
                    Warning3 = RowCharacteristicsMap.Object.ContainsKey(s.GdbArchiveOid) && RowCharacteristicsMap.Object[s.GdbArchiveOid].ViolatedBusinessRules.Intersect(businessRulesForCurrentRecordGroupsWithSameStationId).Any()
                };
            }).ToList();

            //var stationInformationViewModels = _stationInformations.Select(s => new StationInformationViewModel
            //{
            //    StationInformation = s,
            //    Brush = _conditionToBrushMap[RowCharacteristicsMap.Object[s.GdbArchiveOid].RowCondition],
            //    Warning1 = RowCharacteristicsMap.Object.ContainsKey(s.GdbArchiveOid) && RowCharacteristicsMap.Object[s.GdbArchiveOid].ViolatedBusinessRules.Intersect(businessRulesForIndividualRecords).Any(),
            //    Warning2 = RowCharacteristicsMap.Object.ContainsKey(s.GdbArchiveOid) && RowCharacteristicsMap.Object[s.GdbArchiveOid].ViolatedBusinessRules.Intersect(businessRulesForRecordGroupsWithSameObjectId).Any(),
            //    Warning3 = RowCharacteristicsMap.Object.ContainsKey(s.GdbArchiveOid) && RowCharacteristicsMap.Object[s.GdbArchiveOid].ViolatedBusinessRules.Intersect(businessRulesForCurrentRecordGroupsWithSameStationId).Any()
            //}).ToList();

            var objectIdOfPreviousRow = -1;
            var backgroundBrush = _backgroundBrush2;

            foreach (var stationInformationViewModel in stationInformationViewModels)
            {
                if (stationInformationViewModel.StationInformation.ObjectId != objectIdOfPreviousRow)
                {
                    backgroundBrush = backgroundBrush == _backgroundBrush1 ? _backgroundBrush2 : _backgroundBrush1;
                }

                stationInformationViewModel.BackgroundBrush = backgroundBrush;

                objectIdOfPreviousRow = stationInformationViewModel.StationInformation.ObjectId;
            }

            StationInformationViewModels = new ObservableCollection<StationInformationViewModel>(stationInformationViewModels);
        }

        private void SelectionChanged(object commandParameter)
        {
            IList temp = (IList)commandParameter;
            var stationInformationViewModels = temp.Cast<StationInformationViewModel>();

            UpdateStationInformationSelection(stationInformationViewModels.Select(svm => svm.StationInformation));
        }

        private void UpdateStationInformationSelection(IEnumerable<StationInformation> stationInformations)
        {
            SelectedStationInformations.Objects = stationInformations.Sort();
        }

        private void FindStationInformations(
            object owner)
        {
            try
            {
                var limit = 100;

                var count = CountStationInformationsMatchingFilterFromRepository();

                if (count == 0)
                {
                    var dialogViewModel = new MessageBoxDialogViewModel("No station information matches the search criteria", false);
                    _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
                }

                if (count > limit)
                {
                    var dialogViewModel = new MessageBoxDialogViewModel(
                        $"{count} station information rows match the search criteria.\nDo you want to retrieve them all from the repository?", true);

                    if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                FetchStationInformationsFromRepository();
            }
            catch (Exception e)
            {
                var dialogViewModel = new MessageBoxDialogViewModel($"Query failed. Error message: {e.Message}\nDid you provide valid credentials under File->Settings?", false);
                _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
            }
        }

        private void ClearFilters()
        {
            FindStationInformationsViewModel.ClearFilters();
        }

        private void UpdateItemCountText()
        {
            ItemCountText = ItemCount == 1
                ? "1 record"
                : $"{ItemCount} records";
        }

        private void FetchStationInformationsFromRepository()
        {
            _stationInformations = RetrieveStationInformationsMatchingFilterFromRepository();

            // Optionally reduce the data set to records of interest
            if (FindStationInformationsViewModel.CurrentOption != Option.Option1)
            {
                var reducedListOfSelectedStationInformationRecords = new List<StationInformation>();

                foreach (var stationInformation in _stationInformations)
                {
                    var rowCharacteristics = RowCharacteristicsMap.Object[stationInformation.GdbArchiveOid];

                    if (rowCharacteristics.RowCondition == RowCondition.OutDated)
                    {
                        if (rowCharacteristics.FieldsUpdatedInSubsequentRecord.Count == 0)
                        {
                            // Der er ingen ændringer, og vi opererer ikke med ALL, så rækken skal ikke med
                            continue;
                        }
                        else if (
                            FindStationInformationsViewModel.CurrentOption == Option.Option3 &&
                            rowCharacteristics.FieldsUpdatedInSubsequentRecord.Intersect(HistoricallyRelevantFields).Count() == 0)
                        {
                            continue;
                        }
                    }

                    reducedListOfSelectedStationInformationRecords.Add(stationInformation);
                }

                _stationInformations = reducedListOfSelectedStationInformationRecords;
            }

            ItemCount = _stationInformations.Count;

            UpdateStationInformationViewModels();

            // Notify observers that the station information collection has changed
            StationInformations.Objects = _stationInformations;
        }
    }
}
