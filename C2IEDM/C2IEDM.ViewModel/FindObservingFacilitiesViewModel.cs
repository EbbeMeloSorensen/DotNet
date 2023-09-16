using C2IEDM.Domain.Entities;
using GalaSoft.MvvmLight;
using System.Linq.Expressions;
using System;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace C2IEDM.ViewModel;

public class FindObservingFacilitiesViewModel : ViewModelBase
{
    private string _nameFilter = "";
    private string _nameFilterInUppercase = "";

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

    public Expression<Func<ObservingFacility, bool>> FilterAsExpression()
    {
        return _ => _.Superseded == DateTime.MaxValue && _.Name.ToUpper().Contains(_nameFilterInUppercase);
    }
}