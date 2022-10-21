using GalaSoft.MvvmLight;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class PersonAssociationViewModel : ViewModelBase
    {
        private RecordAssociation _personAssociation;
        private bool _isSelected;

        public RecordAssociation PersonAssociation
        {
            get { return _personAssociation; }
            set
            {
                _personAssociation = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayText
        {
            get { return $"{_personAssociation.SubjectRecord.Term} {_personAssociation.Description} {_personAssociation.ObjectRecord.Term}"; }
        }
    }
}
