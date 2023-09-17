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
        ObservableObject<DateTime?> timeOfInterest)
    {
        _timeOfInterest = timeOfInterest;
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