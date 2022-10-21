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

        private int _numberOfPeopleToCreate = 10;

        private RelayCommand _createPeopleCommand;

        public int NumberOfPeopleToCreate
        {
            get { return _numberOfPeopleToCreate; }
            set
            {
                _numberOfPeopleToCreate = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CreatePeopleCommand
        {
            get { return _createPeopleCommand ?? (_createPeopleCommand = new RelayCommand(CreatePeople)); }
        }

        public OptionsDialogViewModel(IUIDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        private void CreatePeople()
        {
            Enumerable.Range(1, NumberOfPeopleToCreate).ToList().ForEach(i =>
            {
                var name = "Person" + i.ToString().PadLeft(4, '0');
                _dataProvider.CreateRecord(new Record { Term = name, Created = DateTime.UtcNow });
            });
        }
    }
}
