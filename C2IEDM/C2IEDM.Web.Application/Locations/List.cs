using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Web.Persistence;

namespace C2IEDM.Web.Application.Locations;

public enum LocationType
{
    Location,
    Point,
    AbsolutePoint
}

public class List
{
    public class Query : IRequest<Result<PagedList<LocationDto>>>
    {
        public LocationType Type { get; set; }
        public LocationParams Params { get; set; }

    }

    public class Handler : IRequestHandler<Query, Result<PagedList<LocationDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;

        public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userAccessor = userAccessor;
        }

        public async Task<Result<PagedList<LocationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<LocationDto> query;

            query = _context.Locations
                //.OrderBy(d => d.FirstName)
                //.ThenByDescending(d => d.Surname == null)
                //.ThenBy(d => d.Surname)
                .ProjectTo<LocationDto>(_mapper.ConfigurationProvider,
                    new { currentUsername = _userAccessor.GetUsername() })
                .AsQueryable();

            return Result<PagedList<LocationDto>>.Success(
                await PagedList<LocationDto>.CreateAsync(query, request.Params.PageNumber,
                    request.Params.PageSize));
        }
    }
}