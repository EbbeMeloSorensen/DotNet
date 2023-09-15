using AutoMapper;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Geometry.DTOs;
using C2IEDM.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Geometry;

public enum LocationCategory
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
        public LocationCategory Category { get; set; }
        public PagingParams Params { get; set; }
        public DateTime? TimeOfInterest { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<LocationDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<LocationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = request.Category switch
            {
                LocationCategory.FanArea => _context.FanAreas.AsQueryable(),
                LocationCategory.CorridorArea => _context.CorridorAreas.AsQueryable(),
                LocationCategory.PolygonArea => _context.PolygonAreas.AsQueryable(),
                LocationCategory.Ellipse => _context.Ellipses.AsQueryable(),
                LocationCategory.Surface => _context.Surfaces.AsQueryable(),
                LocationCategory.Line => _context.Lines.AsQueryable(),
                LocationCategory.RelativePoint => _context.RelativePoints.AsQueryable(),
                LocationCategory.AbsolutePoint => _context.AbsolutePoints.AsQueryable(),
                LocationCategory.Point => _context.Points.AsQueryable(),
                LocationCategory.Location => _context.Locations.AsQueryable(),
                _ => null
            };

            query = request.TimeOfInterest.HasValue
                ? query.Where(_ => _.Created < request.TimeOfInterest.Value && _.Superseded > request.TimeOfInterest.Value)
                : query.Where(_ => _.Superseded == DateTime.MaxValue);

            var count = await query.CountAsync();

            var locations = await query
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            var locationDtos = locations.Select(_ =>
            {
                var temp = _mapper.Map(_, _.GetType(), typeof(LocationDto));

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