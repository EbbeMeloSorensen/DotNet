using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class RecordAssociationsViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;

        private bool _isVisible;
        private ObjectCollection<Record> _records;
        private Record _activeRecord;
        private ObservableCollection<RecordAssociationViewModel> _recordAssociationViewModels;
        private RelayCommand _selectionChangedCommand;
        private RelayCommand _deleteSelectedRecordAssociationsCommand;
        private RelayCommand<object> _createRecordAssociationCommand;
        private RelayCommand<object> _updateRecordAssociationCommand;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public ObjectCollection<RecordAssociation> SelectedRecordAssociations { get; private set; }

        public ObservableCollection<RecordAssociationViewModel> RecordAssociationViewModels
        {
            get { return _recordAssociationViewModels; }
            set
            {
                _recordAssociationViewModels = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand(SelectionChanged)); }
        }

        public RelayCommand DeleteSelectedRecordAssociationsCommand
        {
            get
            {
                return _deleteSelectedRecordAssociationsCommand ?? (
                           _deleteSelectedRecordAssociationsCommand = new RelayCommand(DeleteSelectedRecordAssociations, CanDeleteSelectedRecordAssociations));
            }
        }

        public RelayCommand<object> CreateRecordAssociationCommand
        {
            get
            {
                return _createRecordAssociationCommand ?? (
                           _createRecordAssociationCommand = new RelayCommand<object>(CreateRecordAssociation, CanCreateRecordAssociation));
            }
        }

        public RelayCommand<object> UpdateRecordAssociationCommand
        {
            get
            {
                return _updateRecordAssociationCommand ?? (
                           _updateRecordAssociationCommand = new RelayCommand<object>(UpdateRecordAssociation, CanUpdateRecordAssociation));
            }
        }

        public RecordAssociationsViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService,
            ObjectCollection<Record> records)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;
            _records = records;
            SelectedRecordAssociations = new ObjectCollection<RecordAssociation>();

            // Den her lader til at være tung..
            _records.PropertyChanged += Initialize;
        }

        private void Initialize(object sender, PropertyChangedEventArgs e)
        {
            var temp = sender as ObjectCollection<Record>;

            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                _activeRecord = temp.Objects.Single();
                Populate();
                IsVisible = true;
            }
            else
            {
                _activeRecord = null;
                IsVisible = false;
            }
        }

        private void Populate()
        {
            if (_activeRecord == null)
            {
                RecordAssociationViewModels = new ObservableCollection<RecordAssociationViewModel>();
                return;
            }

            var record = _dataProvider.GetRecordWithAssociations(_activeRecord.Id);

            RecordAssociationViewModels = new ObservableCollection<RecordAssociationViewModel>(record.ObjectRecords
                .Select(pa => new RecordAssociationViewModel
                {
                    RecordAssociation = pa
                })
                .Concat(record.SubjectRecords
                    .Select(pa => new RecordAssociationViewModel
                    {
                        RecordAssociation = pa
                    })));
        }

        private void SelectionChanged()
        {
            SelectedRecordAssociations.Objects = _recordAssociationViewModels
                .Where(p => p.IsSelected)
                .Select(pa => pa.RecordAssociation);

            DeleteSelectedRecordAssociationsCommand.RaiseCanExecuteChanged();
            UpdateRecordAssociationCommand.RaiseCanExecuteChanged();
        }

        public void DeleteSelectedRecordAssociations()
        {
            _dataProvider.DeleteRecordAssociations(SelectedRecordAssociations.Objects.ToList());

            Populate();
        }

        private bool CanDeleteSelectedRecordAssociations()
        {
            return SelectedRecordAssociations.Objects != null &&
                   SelectedRecordAssociations.Objects.Any();
        }

        private void CreateRecordAssociation(object owner)
        {
            var dialogViewModel = new DefineRecordAssociationDialogViewModel(
                _dataProvider,
                _applicationDialogService,
                _activeRecord,
                null,
                null);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            if (_activeRecord != null)
            {
                _dataProvider.CreateRecordAssociation(new RecordAssociation
                {
                    SubjectRecordId = dialogViewModel.SubjectRecord.Id,
                    ObjectRecordId = dialogViewModel.ObjectRecord.Id,
                    Description = dialogViewModel.Description,
                    Created = DateTime.UtcNow
                });

                Populate();
            }
        }

        private bool CanCreateRecordAssociation(object owner)
        {
            return true;
        }

        private void UpdateRecordAssociation(object owner)
        {
            var recordAssociation = SelectedRecordAssociations.Objects.Single();

            var dialogViewModel = new DefineRecordAssociationDialogViewModel(
                _dataProvider,
                _applicationDialogService,
                recordAssociation.SubjectRecord,
                recordAssociation.ObjectRecord,
                recordAssociation.Description);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            recordAssociation.Description = dialogViewModel.Description;
            recordAssociation.SubjectRecordId = dialogViewModel.SubjectRecord.Id;
            recordAssociation.ObjectRecordId = dialogViewModel.ObjectRecord.Id;

            _dataProvider.UpdateRecordAssociation(recordAssociation);

            Populate();
        }

        private bool CanUpdateRecordAssociation(object owner)
        {
            return SelectedRecordAssociations.Objects != null &&
                   SelectedRecordAssociations.Objects.Count() == 1;
        }
    }
}
