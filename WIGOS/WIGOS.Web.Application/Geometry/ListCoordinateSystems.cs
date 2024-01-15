using AutoMapper;
using WIGOS.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry.DTOs;
using WIGOS.Web.Application.Interfaces;

namespace WIGOS.Web.Application.Geometry
{
    public enum CoordinateSystemType
    {
        CoordinateSystem,
        PointReference
    }

    public class ListCoordinateSystems
    {
        public class Query : IRequest<Result<PagedList<CoordinateSystemDto>>>
        {
            public CoordinateSystemType Type { get; set; }
            public PagingParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<CoordinateSystemDto>>>
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

            public async Task<Result<PagedList<CoordinateSystemDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = request.Type switch
                {
                    CoordinateSystemType.PointReference => _context.PointReferences.AsQueryable(),
                    CoordinateSystemType.CoordinateSystem => _context.CoordinateSystems.AsQueryable(),
                    _ => null
                };

                var count = await query.CountAsync();

                var coordinateSystems = await query
                    .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                    .Take(request.Params.PageSize)
                    .ToListAsync();

                var coordinateSystemDtos = coordinateSystems.Select(_ =>
                {
                    var temp = _mapper.Map(_, _.GetType(), typeof(CoordinateSystemDto));

                    if (temp is not CoordinateSystemDto coordinateSystemDto)
                    {
                        throw new InvalidDataException();
                    }

                    return coordinateSystemDto;
                });

                return Result<PagedList<CoordinateSystemDto>>.Success(
                    new PagedList<CoordinateSystemDto>(coordinateSystemDtos, count, request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}