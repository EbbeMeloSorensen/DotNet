using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;

namespace Games.Risk.ViewModel;

public class TransferArmiesDialogViewModel : DialogViewModelBase
{
    private int _armiesToTransfer;
    private RelayCommand<object> _okCommand;

    public int ArmiesToTransfer
    {
        get => _armiesToTransfer;
        set
        {
            _armiesToTransfer = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<int> ArmyTransferOptions { get; }

    public RelayCommand<object> OKCommand
    {
        get { return _okCommand ?? (_okCommand = new RelayCommand<object>(OK)); }
    }

    public TransferArmiesDialogViewModel(
        int minNumberOfArmies,
        int maxNumberOfArmies)
    {
        ArmyTransferOptions = new ObservableCollection<int>(Enumerable.Range(minNumberOfArmies, maxNumberOfArmies - minNumberOfArmies + 1));
        ArmiesToTransfer = minNumberOfArmies;
    }

    private void OK(object parameter)
    {
        CloseDialogWithResult(parameter as Window, DialogResult.OK);
    }
}