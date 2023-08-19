using System;
using System.Linq;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight.Command;
using PR.Application;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.ViewModel
{
    public class OptionsDialogViewModel : DialogViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

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

        public OptionsDialogViewModel(
            IUIDataProvider dataProvider,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _dataProvider = dataProvider;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private void CreatePeople()
        {
            Enumerable.Range(1, NumberOfPeopleToCreate).ToList().ForEach(i =>
            {
                var name = "Person" + i.ToString().PadLeft(4, '0');
                _dataProvider.CreatePerson(new Person { FirstName = name, Created = DateTime.UtcNow });
            });
        }
    }
}
