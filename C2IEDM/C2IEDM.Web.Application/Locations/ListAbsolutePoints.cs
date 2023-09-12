using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.Locations.VerticalDistance;

namespace C2IEDM.Web.Application.Locations;

public class ListAbsolutePoints
{
    public class Query : IRequest<Result<PagedList<AbsolutePointDto>>>
    {
        public PagingParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<AbsolutePointDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<AbsolutePointDto>>> Handle(
            Query request, 
            CancellationToken cancellationToken)
        {
            var query = _context.AbsolutePoints.AsQueryable();

            var count = await query.CountAsync();

            var absolutePoints = await query
                .Where(_ => _.Superseded == null)
                //.Include(_ => _.VerticalDistance)
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            // For en normal database uden historik ville vi bare have et Include statement efter where klausulen
            // for at få parent rækker med. Det duer imidlertid ikke for en database med historik, da parent rækken
            // kan være blevet superseeded med en nyere række.
            // Derfor genererer vi en liste af objekt ids for de parent rækker, vi skal have fat i, og så
            // henter vi disse rækker i et separat kald til databasen.

            var verticalDistanceObjectIds = absolutePoints
                .Where(_ => _.VerticalDistanceObjectId != null)
                .Select(_ => _.VerticalDistanceObjectId)
                .Distinct()
                .ToList();

            var verticalDistances = await  _context.VerticalDistances.AsQueryable()
                .Where(_ => _.Superseded == null && verticalDistanceObjectIds.Contains(_.ObjectId))
                .ToListAsync();

            // Her har vi parent rækkerne i en liste, som vi omdanner til et map for efterfølgende at kunne hæfte dem på de
            // child rækker, vi i første omgang trak ud af databasen.

            var verticalDistanceMap = verticalDistances.ToDictionary(_ => _.ObjectId, _ => _);

            // Her mapper vi det hele til dto instanser og hæfter parents på

            var absolutePointDtos = absolutePoints.Select(_ =>
            {
                var absolutePointDto = _mapper.Map(_, _.GetType(), typeof(AbsolutePointDto)) as AbsolutePointDto;

                if (_.VerticalDistanceObjectId.HasValue &&
                    verticalDistanceMap.TryGetValue(_.VerticalDistanceObjectId.Value, out var verticalDistance))
                {
                    var verticalDistanceDto = _mapper.Map(
                        verticalDistance, 
                        typeof(Domain.Entities.Geometry.VerticalDistance), 
                        typeof(VerticalDistanceDto)) as VerticalDistanceDto;

                    absolutePointDto.VerticalDistance = verticalDistanceDto;
                }

                return absolutePointDto;
            });

            return Result<PagedList<AbsolutePointDto>>.Success(
                new PagedList<AbsolutePointDto>(absolutePointDtos, count, request.Params.PageNumber, request.Params.PageSize));
        }
    }
}