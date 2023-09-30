using System;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using Craft.Utils;

namespace C2IEDM.ViewModel;

public class FindObservingFacilitiesViewModel : ViewModelBase
{
    private string _nameFilter = "";
    private string _nameFilterInUppercase = "";
    private readonly ObservableObject<DateTime?> _timeOfInterest;
    private readonly ObservableObject<bool> _displayHistoricalTimeControls;
    private readonly ObservableObject<bool> _displayDatabaseTimeControls;
    private bool _displayRetrospectionControlSection;
    private bool _displayHistoricalTimeField;
    private bool _displayDatabaseTimeField;

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

    public FindObservingFacilitiesViewModel(
        ObservableObject<DateTime?> timeOfInterest,
        ObservableObject<bool> displayHistoricalTimeControls,
        ObservableObject<bool> displayDatabaseTimeControls)
    {
        _timeOfInterest = timeOfInterest;
        _displayHistoricalTimeControls = displayHistoricalTimeControls;
        _displayDatabaseTimeControls = displayDatabaseTimeControls;

        UpdateRetrospectionControlGroup();

        _displayHistoricalTimeControls.PropertyChanged += (s, e) => UpdateRetrospectionControlGroup();
        _displayDatabaseTimeControls.PropertyChanged += (s, e) => UpdateRetrospectionControlGroup();
    }

    private void UpdateRetrospectionControlGroup()
    {
        DisplayRetrospectionControlSection =
            _displayHistoricalTimeControls.Object ||
            _displayDatabaseTimeControls.Object;

        DisplayHistoricalTimeField = _displayHistoricalTimeControls.Object;
        DisplayDatabaseTimeField = _displayDatabaseTimeControls.Object;
    }

    public Expression<Func<ObservingFacility, bool>> FilterAsExpression()
    {
        if (_timeOfInterest.Object.HasValue)
        {
            return _ =>
                _.Created <= _timeOfInterest.Object.Value && 
                _.Superseded > _timeOfInterest.Object.Value && 
                _.Name.ToUpper().Contains(_nameFilterInUppercase);
        }

        return _ => 
            _.Superseded == DateTime.MaxValue && 
            _.Name.ToUpper().Contains(_nameFilterInUppercase);
    }
}