using GalaSoft.MvvmLight;
using PR.Domain.Entities.PR;

namespace PR.ViewModel;

public class PersonVariantListViewItemViewModel : ViewModelBase
{
    private Person _personVariant;

    public Person PersonVariant
    {
        get { return _personVariant; }
        set
        {
            _personVariant = value;
            RaisePropertyChanged();
        }
    }

    public string DisplayText
    {
        get { return $"{_personVariant.FirstName} ({_personVariant.Start} - {_personVariant.End}"; }
    }
}