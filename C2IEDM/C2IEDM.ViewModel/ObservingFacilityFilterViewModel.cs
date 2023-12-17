using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;

namespace C2IEDM.ViewModel;

public class ObservingFacilityFilterViewModel : ViewModelBase
{
    private string _nameFilter = "";
    private bool _showActiveObservingFacilities;
    private bool _showClosedObservingFacilities;
    private bool _showActiveObservingFacilitiesCheckboxEnabled;
    private bool _showClosedObservingFacilitiesCheckboxEnabled;

    private readonly ObservableObject<DateTime?> _historicalTimeOfInterest;
    private readonly ObservableObject<DateTime?> _databaseTimeOfInterest;
    private readonly ObservableObject<bool> _displayNameFilter;
    private readonly ObservableObject<bool> _displayStatusFilter;
    private readonly ObservableObject<bool> _showActiveStations;
    private readonly ObservableObject<bool> _showClosedStations;
    private readonly ObservableObject<bool> _displayHistoricalTimeControls;
    private readonly ObservableObject<bool> _displayDatabaseTimeControls;
    private bool _displayNameFilterField;
    private bool _displayStatusFilterSection;
    private bool _displayRetrospectionControlSection;
    private bool _displayHistoricalTimeField;
    private bool _displayDatabaseTimeField;
    private string _historicalTimeOfInterestAsString;
    private string _databaseTimeOfInterestAsString;

    private RelayCommand _clearHistoricalTimeCommand;
    private RelayCommand _clearDatabaseTimeCommand;

    public bool DisplayNameFilterField
    {
        get => _displayNameFilterField;
        set
        {
            _displayNameFilterField = value;
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

    public bool DisplayRetrospectionControlSection
    {
        get => _displayRetrospectionControlSection;
        set
        {
            _displayRetrospectionControlSection = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayHistoricalTimeField
    {
        get => _displayHistoricalTimeField;
        set
        {
            _displayHistoricalTimeField = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayDatabaseTimeField
    {
        get => _displayDatabaseTimeField;
        set
        {
            _displayDatabaseTimeField = value;
            RaisePropertyChanged();
        }
    }

    public string NameFilter
    {
        get { return _nameFilter; }
        set
        {
            _nameFilter = value;
            RaisePropertyChanged();
        }
    }

    public bool ShowActiveObservingFacilities
    {
        get { return _showActiveObservingFacilities; }
        set
        {
            _showActiveObservingFacilities = value;
            _showActiveStations.Object = value;
            RaisePropertyChanged();
            UpdateControls();
        }
    }

    public bool ShowClosedObservingFacilities
    {
        get { return _showClosedObservingFacilities; }
        set
        {
            _showClosedObservingFacilities = value;
            _showClosedStations.Object = value;
            RaisePropertyChanged();
            UpdateControls();
        }
    }

    public bool ShowActiveObservingFacilitiesCheckboxEnabled
    {
        get { return _showActiveObservingFacilitiesCheckboxEnabled; }
        set
        {
            _showActiveObservingFacilitiesCheckboxEnabled = value;
            RaisePropertyChanged();
        }
    }

    public bool ShowClosedObservingFacilitiesCheckboxEnabled
    {
        get { return _showClosedObservingFacilitiesCheckboxEnabled; }
        set
        {
            _showClosedObservingFacilitiesCheckboxEnabled = value;
            RaisePropertyChanged();
        }
    }

    public string HistoricalTimeOfInterestAsString
    {
        get => _historicalTimeOfInterestAsString;
        set
        {
            _historicalTimeOfInterestAsString = value;
            RaisePropertyChanged();
        }
    }

    public string DatabaseTimeOfInterestAsString
    {
        get => _databaseTimeOfInterestAsString;
        set
        {
            _databaseTimeOfInterestAsString = value;
            RaisePropertyChanged();
        }
    }

    public RelayCommand ClearHistoricalTimeCommand
    {
        get
        {
            return _clearHistoricalTimeCommand ?? (_clearHistoricalTimeCommand =
                new RelayCommand(ClearHistoricalTime, CanClearHistoricalTime));
        }
    }

    public RelayCommand ClearDatabaseTimeCommand
    {
        get
        {
            return _clearDatabaseTimeCommand ??
                   (_clearDatabaseTimeCommand = new RelayCommand(ClearDatabaseTime, CanClearDatabaseTime));
        }
    }

    public ObservingFacilityFilterViewModel(
        ObservableObject<DateTime?> historicalTimeOfInterest,
        ObservableObject<DateTime?> databaseTimeOfInterest,
        ObservableObject<bool> displayNameFilter,
        ObservableObject<bool> displayStatusFilter,
        ObservableObject<bool> showActiveStations,
        ObservableObject<bool> showClosedStations,
        ObservableObject<bool> displayHistoricalTimeControls,
        ObservableObject<bool> displayDatabaseTimeControls)
    {
        _historicalTimeOfInterest = historicalTimeOfInterest;
        _databaseTimeOfInterest = databaseTimeOfInterest;
        _displayNameFilter = displayNameFilter;
        _displayStatusFilter = displayStatusFilter;
        _showActiveStations = showActiveStations;
        _showClosedStations = showClosedStations;
        _displayHistoricalTimeControls = displayHistoricalTimeControls;
        _displayDatabaseTimeControls = displayDatabaseTimeControls;
        _showActiveObservingFacilities = _showActiveStations.Object;
        _showClosedObservingFacilities = _showClosedStations.Object;

        _historicalTimeOfInterestAsString = "Now";
        _databaseTimeOfInterestAsString = "Latest";

        _historicalTimeOfInterest.PropertyChanged += (s, e) =>
        {
            if (_historicalTimeOfInterest.Object.HasValue)
            {
                HistoricalTimeOfInterestAsString = _historicalTimeOfInterest.Object.Value.AsDateTimeString(false);
            }
            else
            {
                HistoricalTimeOfInterestAsString = "Now";
            }

            ClearHistoricalTimeCommand.RaiseCanExecuteChanged();
        };

        _databaseTimeOfInterest.PropertyChanged += (s, e) =>
        {
            if (_databaseTimeOfInterest.Object.HasValue)
            {
                DatabaseTimeOfInterestAsString = _databaseTimeOfInterest.Object.Value.AsDateTimeString(false);
            }
            else
            {
                DatabaseTimeOfInterestAsString = "Latest";
            }

            ClearDatabaseTimeCommand.RaiseCanExecuteChanged();
        };

        _displayNameFilter.PropertyChanged += (s, e) => UpdateControls();
        _displayStatusFilter.PropertyChanged += (s, e) => UpdateControls();
        _displayHistoricalTimeControls.PropertyChanged += (s, e) => UpdateControls();
        _displayDatabaseTimeControls.PropertyChanged += (s, e) => UpdateControls();

        UpdateControls();
    }

    private void UpdateControls()
    {
        DisplayNameFilterField = _displayNameFilter.Object;
        DisplayStatusFilterSection = _displayStatusFilter.Object;

        DisplayRetrospectionControlSection =
            _displayHistoricalTimeControls.Object ||
            _displayDatabaseTimeControls.Object;

        DisplayHistoricalTimeField = _displayHistoricalTimeControls.Object;
        DisplayDatabaseTimeField = _displayDatabaseTimeControls.Object;

        ShowActiveObservingFacilitiesCheckboxEnabled = ShowClosedObservingFacilities;
        ShowClosedObservingFacilitiesCheckboxEnabled = ShowActiveObservingFacilities;
    }

    private void ClearHistoricalTime()
    {
        _historicalTimeOfInterest.Object = null;
    }

    private bool CanClearHistoricalTime()
    {
        return _historicalTimeOfInterest.Object.HasValue;
    }

    private void ClearDatabaseTime()
    {
        _databaseTimeOfInterest.Object = null;
    }

    private bool CanClearDatabaseTime()
    {
        return _databaseTimeOfInterest.Object.HasValue;
    }
}
 