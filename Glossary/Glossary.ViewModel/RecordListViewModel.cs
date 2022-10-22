using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class RecordListViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private IList<Record> _records;
        private Sorting _sorting;

        public FindRecordsViewModel FindRecordsViewModel { get; private set; }
        private ObservableCollection<RecordViewModel> _recordViewModels;

        private RelayCommand<object> _selectionChangedCommand;
        private RelayCommand<object> _findRecordsCommand;

        public ObservableCollection<RecordViewModel> RecordViewModels
        {
            get { return _recordViewModels; }
            set
            {
                _recordViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObjectCollection<Record> SelectedRecords { get; private set; }

        public Sorting Sorting
        {
            get { return _sorting; }
            set
            {
                _sorting = value;
                RaisePropertyChanged();
                UpdateSorting();
                UpdateRecordViewModels();
            }
        }

        public RelayCommand<object> SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<object>(SelectionChanged)); }
        }

        public RelayCommand<object> FindRecordsCommand
        {
            get
            {
                return _findRecordsCommand ?? (_findRecordsCommand = new RelayCommand<object>(FindRecords));
            }
        }

        public RecordListViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;
            _sorting = Sorting.Name;

            FindRecordsViewModel = new FindRecordsViewModel();

            _records = new List<Record>();

            SelectedRecords = new ObjectCollection<Record>();

            dataProvider.RecordCreated += (s, e) =>
            {
                if (!FindRecordsViewModel.RecordPassesFilter(e.Record))
                {
                    return;
                }

                _records.Add(e.Record);
                UpdateRecordViewModels();
            };

            dataProvider.RecordsUpdated += (s, e) =>
            {
                UpdateRecordViewModels();
            };

            dataProvider.RecordsDeleted += (s, e) =>
            {
                var countBefore = _records.Count;
                _records = _records.Except(e.Records).ToList();
                var countAfter = _records.Count;

                if (countAfter < countBefore)
                {
                    UpdateRecordViewModels();
                }
            };
        }

        private void RetrieveRecordsMatchingFilterFromRepository()
        {
            _records = _dataProvider.FindRecords(FindRecordsViewModel.FilterAsExpression());
        }

        private int CountRecordsMatchingFilterFromRepository()
        {
            return _dataProvider.CountRecords(FindRecordsViewModel.FilterAsExpression());
        }

        private void UpdateSorting()
        {
            switch (Sorting)
            {
                case Sorting.Name:
                    _records = _records.OrderBy(p => p.Term).ToList();
                    break;
                case Sorting.Created:
                    _records = _records.OrderByDescending(p => p.Created).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateRecordViewModels()
        {
            UpdateSorting();

            RecordViewModels = new ObservableCollection<RecordViewModel>(_records.Select(p => new RecordViewModel
            {
                Record = p
            }));
        }

        private void SelectionChanged(object commandParameter)
        {
            IList temp = (IList)commandParameter;
            var recordViewModels = temp.Cast<RecordViewModel>();

            UpdateRecordSelection(recordViewModels.Select(rvm => rvm.Record));
        }

        private void UpdateRecordSelection(IEnumerable<Record> records)
        {
            SelectedRecords.Objects = records;
        }

        private void FindRecords(object owner)
        {
            var recordLimit = 10;
            var count = CountRecordsMatchingFilterFromRepository();

            if (count == 0)
            {
                var dialogViewModel = new MessageBoxDialogViewModel("No record matches the search criteria", false);
                _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
            }

            if (count > recordLimit)
            {
                var dialogViewModel = new MessageBoxDialogViewModel($"{count} records match the search criteria.\nDo you want to retrieve them all from the repository?", true);
                if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
                {
                    return;
                }
            }

            RetrieveRecordsMatchingFilterFromRepository();
            UpdateRecordViewModels();
        }
    }
}
