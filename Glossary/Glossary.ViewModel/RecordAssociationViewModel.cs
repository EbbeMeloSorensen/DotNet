using GalaSoft.MvvmLight;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class RecordAssociationViewModel : ViewModelBase
    {
        private RecordAssociation _recordAssociation;
        private bool _isSelected;

        public RecordAssociation RecordAssociation
        {
            get { return _recordAssociation; }
            set
            {
                _recordAssociation = value;
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
            get { return $"{_recordAssociation.SubjectRecord.Term} {_recordAssociation.Description} {_recordAssociation.ObjectRecord.Term}"; }
        }
    }
}
