using System;
using System.Linq;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using Glossary.Application;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class OptionsDialogViewModel : DialogViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;

        private int _numberOfRecordsToCreate = 10;

        private RelayCommand _createRecordsCommand;

        public int NumberOfRecordsToCreate
        {
            get { return _numberOfRecordsToCreate; }
            set
            {
                _numberOfRecordsToCreate = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CreateRecordsCommand
        {
            get { return _createRecordsCommand ?? (_createRecordsCommand = new RelayCommand(CreateRecords)); }
        }

        public OptionsDialogViewModel(IUIDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        private void CreateRecords()
        {
            Enumerable.Range(1, NumberOfRecordsToCreate).ToList().ForEach(i =>
            {
                var name = "Record" + i.ToString().PadLeft(4, '0');
                _dataProvider.CreateRecord(new Record { Term = name, Created = DateTime.UtcNow });
            });
        }
    }
}
