using System.Collections.ObjectModel;
using System.Linq;
using Craft.ViewModels.Dialogs;

namespace Games.Risk.ViewModel;

public class TransferArmiesDialogViewModel : DialogViewModelBase
{
    private int _armiesToTransfer;

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

    public TransferArmiesDialogViewModel(
        int maxNumberOfArmies)
    {
        ArmyTransferOptions = new ObservableCollection<int>(Enumerable.Range(1, maxNumberOfArmies));
        ArmiesToTransfer = maxNumberOfArmies;
    }
}