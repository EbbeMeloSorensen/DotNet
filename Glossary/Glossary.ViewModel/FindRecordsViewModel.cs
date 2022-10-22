using System;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using Glossary.Domain.Entities;

namespace Glossary.ViewModel
{
    public class FindRecordsViewModel : ViewModelBase
    {
        private string _nameFilter = "";
        private string _nameFilterInUppercase = "";
        private string _categoryFilter = "";
        private string _categoryFilterInUppercase = "";

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

        public Expression<Func<Record, bool>> FilterAsExpression()
        {
            return p => p.Term.ToUpper().Contains(_nameFilterInUppercase) &&
                        (p.Category == null && _categoryFilterInUppercase == "" ||
                         p.Category != null && p.Category.ToUpper().Contains(_categoryFilterInUppercase));
        }

        public bool RecordPassesFilter(Record record)
        {
            var nameOK = string.IsNullOrEmpty(NameFilter) ||
                         record.Term.ToUpper().Contains(NameFilter.ToUpper());

            var categoryOK = string.IsNullOrEmpty(CategoryFilter) ||
                             record.Category != null && record.Category.ToUpper().Contains(CategoryFilter.ToUpper());

            return nameOK && categoryOK;
        }
    }
}
