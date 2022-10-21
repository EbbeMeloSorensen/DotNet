using GalaSoft.MvvmLight;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel;

public class PersonViewModel : ViewModelBase
{
    private Record _person;

    public Record Person
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
            var displayText = _person.Term;

            return displayText;
        }
    }
}