using GalaSoft.MvvmLight;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel;

public class RecordViewModel : ViewModelBase
{
    private Record _record;

    public Record Record
    {
        get { return _record; }
        set
        {
            _record = value;
            RaisePropertyChanged();
        }
    }

    public string DisplayText
    {
        get
        {
            var displayText = _record.Term;

            return displayText;
        }
    }
}