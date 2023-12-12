using System;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using Craft.Utils;
using GalaSoft.MvvmLight.Command;

namespace C2IEDM.ViewModel;

public class ObservingFacilityFilterViewModel : ViewModelBase
{
    private string _nameFilter = "";
    private string _nameFilterInUppercase = "";
    private readonly ObservableObject<DateTime?> _historicalTimeOfInterest;
    private readonly ObservableObject<DateTime?> _databaseTimeOfInterest;
    private readonly ObservableObject<bool> _displayNameFilter;
    private readonly ObservableObject<bool> _displayHistoricalTimeControls;
    private readonly ObservableObject<bool> _displayDatabaseTimeControls;
    private bool _displayNameFilterField;
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

            _nameFilterInUppercase = _nameFilter == null ? "" : _nameFilter.ToUpper();
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
            return _clearHistoricalTimeCommand ?? (_clearHistoricalTimeCommand = new RelayCommand(ClearHistoricalTime, CanClearHistoricalTime));
        }
    }

    public RelayCommand ClearDatabaseTimeCommand
    {
        get
        {
            return _clearDatabaseTimeCommand ?? (_clearDatabaseTimeCommand = new RelayCommand(ClearDatabaseTime, CanClearDatabaseTime));
        }
    }

    public ObservingFacilityFilterViewModel(
        ObservableObject<DateTime?> historicalTimeOfInterest,
        ObservableObject<DateTime?> databaseTimeOfInterest,
        ObservableObject<bool> displayNameFilter,
        ObservableObject<bool> displayHistoricalTimeControls,
        ObservableObject<bool> displayDatabaseTimeControls)
    {
        _historicalTimeOfInterest = historicalTimeOfInterest;
        _databaseTimeOfInterest = databaseTimeOfInterest;
        _displayNameFilter = displayNameFilter;
        _displayHistoricalTimeControls = displayHistoricalTimeControls;
        _displayDatabaseTimeControls = displayDatabaseTimeControls;

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

        _displayNameFilter.PropertyChanged += (s, e) => UpdateFilterControls();
        _displayHistoricalTimeControls.PropertyChanged += (s, e) => UpdateRetrospectionControlGroup();
        _displayDatabaseTimeControls.PropertyChanged += (s, e) => UpdateRetrospectionControlGroup();

        UpdateRetrospectionControlGroup();
    }

    private void UpdateFilterControls()
    {
        DisplayNameFilterField = _displayNameFilter.Object;
    }

    private void UpdateRetrospectionControlGroup()
    {
        DisplayRetrospectionControlSection =
            _displayHistoricalTimeControls.Object ||
            _displayDatabaseTimeControls.Object;

        DisplayHistoricalTimeField = _displayHistoricalTimeControls.Object;
        DisplayDatabaseTimeField = _displayDatabaseTimeControls.Object;
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