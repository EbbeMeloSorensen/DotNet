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
    private bool _displayRetrospectionControls;

    public bool DisplayRetrospectionControls
    {
        get => _displayRetrospectionControls;
        set
        {
            _displayRetrospectionControls = value;
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
        ObservableObject<bool> displayRetrospectionControls)
    {
        _timeOfInterest = timeOfInterest;

        displayRetrospectionControls.PropertyChanged += (s, e) =>
        {
            if (s is ObservableObject<bool> temp)
            {
                DisplayRetrospectionControls = temp.Object;
            }
        };
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