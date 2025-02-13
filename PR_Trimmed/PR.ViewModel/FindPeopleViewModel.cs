using System;
using System.Linq.Expressions;
using Craft.Utils;
using GalaSoft.MvvmLight;
using PR.Domain.Entities.PR;

namespace PR.ViewModel
{
    public class FindPeopleViewModel : ViewModelBase
    {
        private bool _displayAttributeFilterSection;
        private bool _displayStatusFilterSection;
        private bool _displayRetrospectiveFilterSection;
        private bool _displayHistoricalTimeControls;
        private bool _displayDatabaseTimeControls;
        private string _nameFilter = "";
        private string _nameFilterInUppercase = "";
        private string _categoryFilter = "";
        private string _categoryFilterInUppercase = "";

        private bool _showCurrentPeople;
        private bool _showHistoricalPeople;
        private bool _showCurrentPeopleCheckboxEnabled;
        private bool _showHistoricalPeopleCheckboxEnabled;

        public bool DisplayAttributeFilterSection
        {
            get => _displayAttributeFilterSection;
            set
            {
                _displayAttributeFilterSection = value;
                RaisePropertyChanged();
            }
        }

        public bool DisplayStatusFilterSection
        {
            get => _displayStatusFilterSection;
            set
            {
                _displayStatusFilterSection = value;
                RaisePropertyChanged();
            }
        }

        public bool DisplayRetrospectiveFilterSection
        {
            get => _displayRetrospectiveFilterSection;
            set
            {
                _displayRetrospectiveFilterSection = value;
                RaisePropertyChanged();
            }
        }

        public bool DisplayHistoricalTimeControls
        {
            get => _displayHistoricalTimeControls;
            set
            {
                _displayHistoricalTimeControls = value;
                RaisePropertyChanged();

                DisplayRetrospectiveFilterSection =
                    DisplayHistoricalTimeControls ||
                    DisplayDatabaseTimeControls;
            }
        }

        public bool DisplayDatabaseTimeControls
        {
            get => _displayDatabaseTimeControls;
            set
            {
                _displayDatabaseTimeControls = value;
                RaisePropertyChanged();

                DisplayRetrospectiveFilterSection =
                    DisplayHistoricalTimeControls ||
                    DisplayDatabaseTimeControls;
            }
        }

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

        public bool ShowCurrentPeople
        {
            get { return _showCurrentPeople; }
            set
            {
                _showCurrentPeople = value;
                RaisePropertyChanged();

                ShowCurrentPeopleCheckboxEnabled = ShowHistoricalPeople;
                ShowHistoricalPeopleCheckboxEnabled = ShowCurrentPeople;
            }
        }

        public bool ShowHistoricalPeople
        {
            get { return _showHistoricalPeople; }
            set
            {
                _showHistoricalPeople = value;
                RaisePropertyChanged();

                ShowCurrentPeopleCheckboxEnabled = ShowHistoricalPeople;
                ShowHistoricalPeopleCheckboxEnabled = ShowCurrentPeople;
            }
        }

        public bool ShowCurrentPeopleCheckboxEnabled
        {
            get { return _showCurrentPeopleCheckboxEnabled; }
            set
            {
                _showCurrentPeopleCheckboxEnabled = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowHistoricalPeopleCheckboxEnabled
        {
            get { return _showHistoricalPeopleCheckboxEnabled; }
            set
            {
                _showHistoricalPeopleCheckboxEnabled = value;
                RaisePropertyChanged();
            }
        }

        public FindPeopleViewModel()
        {
            DisplayAttributeFilterSection = true;
            DisplayStatusFilterSection = true;
            ShowCurrentPeople = true;
            DisplayRetrospectiveFilterSection = true;
            DisplayHistoricalTimeControls = true;
            DisplayDatabaseTimeControls = true;
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
