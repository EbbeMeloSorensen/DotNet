using MediatR;
using AutoMapper;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Web.Persistence;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Locations;

public enum LocationType
{
    Location,
    Point,
    AbsolutePoint,
    RelativePoint,
    Line,
    Surface,
    Ellipse,
    CorridorArea,
    PolygonArea,
    FanArea
}

public class ListLocations
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
            var query = request.Type switch
            {
                LocationType.FanArea => _context.FanAreas.AsQueryable(),
                LocationType.CorridorArea => _context.CorridorAreas.AsQueryable(),
                LocationType.PolygonArea => _context.PolygonAreas.AsQueryable(),
                LocationType.Ellipse => _context.Ellipses.AsQueryable(),
                LocationType.Surface => _context.Surfaces.AsQueryable(),
                LocationType.Line => _context.Lines.AsQueryable(),
                LocationType.RelativePoint => _context.RelativePoints.AsQueryable(),
                LocationType.AbsolutePoint => _context.AbsolutePoints.AsQueryable(),
                LocationType.Point => _context.Points.AsQueryable(),
                LocationType.Location => _context.Locations.AsQueryable(),
                _ => null
            };

            var count = await query.CountAsync();

            var locations = await query
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            var locationDtos = locations.Select(l =>
            {
                var temp = _mapper.Map(l, l.GetType(), typeof(LocationDto));

                if (temp is not LocationDto locationDto)
                {
                    throw new InvalidDataException();
                }

                return locationDto;
            });

            return Result<PagedList<LocationDto>>.Success(
                new PagedList<LocationDto>(locationDtos, count, request.Params.PageNumber, request.Params.PageSize));

            /*
            // Det her var det oprindelige, der jo altså ikke virker polymorfisk
            // Bemærk, at den bruger den der userAccessor - det ved jeg ikke hvorfor...
            IQueryable<LocationDto> query;

            query = _context.Locations
                .ProjectTo<LocationDto>(_mapper.ConfigurationProvider,
                    new { currentUsername = _userAccessor.GetUsername() })
                .AsQueryable();

            var count = await query.CountAsync();

            var locationDtos = await query
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            var pagedList =
                new PagedList<LocationDto>(locationDtos, count, request.Params.PageNumber, request.Params.PageSize);

            return Result<PagedList<LocationDto>>.Success(pagedList);
            */
        }
    }
}