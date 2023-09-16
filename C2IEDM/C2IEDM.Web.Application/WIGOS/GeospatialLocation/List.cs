using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.WIGOS.DTOs;
using C2IEDM.Web.Persistence;

namespace C2IEDM.Web.Application.WIGOS.GeospatialLocation;

public enum GeospatialLocationCategory
{
    GeospatialLocation,
    Point
}

public class List
{
    public class Query : IRequest<Result<PagedList<GeospatialLocationDto>>>
    {
        public GeospatialLocationCategory Category { get; set; }
        public PagingParams Params { get; set; }
        public DateTime? TimeOfInterest { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<GeospatialLocationDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<GeospatialLocationDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var query = request.Category switch
            {
                GeospatialLocationCategory.Point => _context.Points_WIGOS.AsQueryable(),
                GeospatialLocationCategory.GeospatialLocation => _context.GeoSpatialLocations.AsQueryable(),
                _ => null
            };

            query = request.TimeOfInterest.HasValue
                ? query.Where(_ => _.Created < request.TimeOfInterest.Value && _.Superseded > request.TimeOfInterest.Value)
                : query.Where(_ => _.Superseded == DateTime.MaxValue);

            var count = await query.CountAsync();

            var geospatialLocations = await query
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            var geospatialLocationDtos = geospatialLocations.Select(_ =>
            {
                var temp = _mapper.Map(_, _.GetType(), typeof(GeospatialLocationDto));

                if (temp is not GeospatialLocationDto geospatialLocationDto)
                {
                    throw new InvalidDataException();
                }

                return geospatialLocationDto;
            });

            return Result<PagedList<GeospatialLocationDto>>.Success(
                new PagedList<GeospatialLocationDto>(geospatialLocationDtos, count, request.Params.PageNumber, request.Params.PageSize));
        }
    }
}