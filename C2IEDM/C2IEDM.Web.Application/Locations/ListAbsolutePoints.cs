using MediatR;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;

namespace C2IEDM.Web.Application.Locations;

public class ListAbsolutePoints
{
    public class Query : IRequest<Result<PagedList<LocationDto>>>
    {
        public LocationCategory Category { get; set; }
        public LocationParams Params { get; set; }
    }
}