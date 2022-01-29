using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using DMI.SMS.Application;

namespace DMI.SMS.ViewModel
{
    public class BusinessRuleViolationListViewModel : ViewModelBase
    {
        private Dictionary<BusinessRule, string> _businessRuleDescriptionMap = new Dictionary<BusinessRule, string>
        {
            { BusinessRule.ACurrentRowMustHaveAStationName, "A current row should have a station name."},
            { BusinessRule.ACurrentRowMustNotHaveAStationNameWrittenInUpperCase, "A current row should not have a station name written in uppercase"},
            { BusinessRule.ACurrentRowMustHaveACountry, "A current row should have a country."},
            { BusinessRule.ACurrentRowMustHaveAStationType, "A current row should have a station type."},
            { BusinessRule.ACurrentRowMustHaveAStationOwner, "A current row should have a station owner."},
            { BusinessRule.ACurrentRowMustHaveAStatus, "A current row should have a status."},
            { BusinessRule.ACurrentRowMustHaveWGSCoordinates, "A current row should have wgs coordinates"},
            //{ BusinessRule.ACurrentRowMustHaveAHeight, "A current row should have a height"},
            //{ BusinessRule.ACurrentRowWithStationTypeSynopMustHaveABarometerHeight, "A current row with station type: Synop should have a barometer height"},
            { BusinessRule.ACurrentRowWithStatusInactiveMustHaveADateTo, "A current row with status: Inactive should have a date to"},
            { BusinessRule.ObjectWasSubjectedToChangeOfNameSinceCreation, "Station Name was changed between DateFrom and DateTo"},
            { BusinessRule.ObjectWasSubjectedToChangeOfLocationSinceCreation, "Station Location was changed between DateFrom and DateTo"},
            { BusinessRule.ObjectWasSubjectedToChangeOfHhaSinceCreation, "Station Hha (height) was changed between DateFrom and DateTo"},
            { BusinessRule.ObjectWasSubjectedToChangeOfHhpSinceCreation, "Station Hhp (barometer height) was changed between DateFrom and DateTo"},
            { BusinessRule.OverlappingCurrentRecordsWithSameStationIdExists, "Overlapping current records with same station id exists"},
        };

        private bool _isVisible;
        private ObservableCollection<string> _violatedBusinessRules;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> ViolatedBusinessRules
        {
            get
            {
                return _violatedBusinessRules;
            }
            set
            {
                _violatedBusinessRules = value;
                RaisePropertyChanged();
            }
        }

        public BusinessRuleViolationListViewModel()
        {
            ViolatedBusinessRules = new ObservableCollection<string>();
        }

        public void Populate(
            IEnumerable<BusinessRule> violatedBusinessRules)
        {
            ViolatedBusinessRules = new ObservableCollection<string>(
                violatedBusinessRules.Select(b => _businessRuleDescriptionMap[b]));

            IsVisible = ViolatedBusinessRules.Any();
        }

        public void Clear()
        {
            ViolatedBusinessRules = new ObservableCollection<string>();

            IsVisible = false;
        }
    }
}
