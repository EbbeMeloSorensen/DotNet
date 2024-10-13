using GalaSoft.MvvmLight;
using PR.Domain.Entities;

namespace PR.ViewModel;

public class PersonViewModel : ViewModelBase
{
    private Person _person;

    public Person Person
    {
        get { return _person; }
        set
        {
            _person = value;
            RaisePropertyChanged();
        }
    }

    public string DisplayText
    {
        get
        {
            var displayText = _person.FirstName;
            if (_person.Surname != null)
            {
                displayText += $" {_person.Surname}";
            }

            return displayText;
        }
    }
}