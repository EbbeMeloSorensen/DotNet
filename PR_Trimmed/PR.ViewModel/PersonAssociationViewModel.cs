using GalaSoft.MvvmLight;
using PR.Domain.Entities;

namespace PR.ViewModel
{
    public class PersonAssociationViewModel : ViewModelBase
    {
        private PersonAssociation _personAssociation;
        private bool _isSelected;

        public PersonAssociation PersonAssociation
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
            get { return $"{_personAssociation.SubjectPerson.FirstName} {_personAssociation.Description} {_personAssociation.ObjectPerson.FirstName}"; }
        }
    }
}
