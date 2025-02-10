using System;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using PR.Domain.Entities.PR;

namespace PR.ViewModel
{
    public class FindPeopleViewModel : ViewModelBase
    {
        private string _nameFilter = "";
        private string _nameFilterInUppercase = "";
        private string _categoryFilter = "";
        private string _categoryFilterInUppercase = "";

        private bool _displayAttributeFilterSection;

        public string NameFilter
        {
            get { return _nameFilter; }
            set
            {
                _nameFilter = value;

                _nameFilterInUppercase = _nameFilter == null ? "" : _nameFilter.ToUpper();
                RaisePropertyChanged();
            }
        }

        public string CategoryFilter
        {
            get { return _categoryFilter; }
            set
            {
                _categoryFilter = value;
                _categoryFilterInUppercase = _categoryFilter == null ? "" : _categoryFilter.ToUpper();
                RaisePropertyChanged();
            }
        }

        public bool DisplayAttributeFilterSection
        {
            get => _displayAttributeFilterSection;
            set
            {
                _displayAttributeFilterSection = value;
                RaisePropertyChanged();
            }
        }

        public Expression<Func<Person, bool>> FilterAsExpression()
        {
            return p => (p.FirstName.ToUpper().Contains(_nameFilterInUppercase) ||
                         p.Surname != null && p.Surname.ToUpper().Contains(_nameFilterInUppercase));
        }

        public bool PersonPassesFilter(Person person)
        {
            var nameOK = string.IsNullOrEmpty(NameFilter) ||
                         person.FirstName.ToUpper().Contains(NameFilter.ToUpper()) ||
                         person.Surname != null && person.Surname.ToUpper().Contains(NameFilter.ToUpper());

            return nameOK;
        }
    }
}
