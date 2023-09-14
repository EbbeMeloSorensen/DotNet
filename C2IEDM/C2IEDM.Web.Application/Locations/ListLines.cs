using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.Locations.VerticalDistance;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Domain.Entities.Geometry.Locations.Line;

namespace C2IEDM.Web.Application.Locations;

public class ListLines
{
    public class Query : IRequest<Result<PagedList<LineDto>>>
    {
        public PagingParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<LineDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<LineDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var query = _context.Lines.AsQueryable();

            var count = await query.CountAsync();

            var lines = await query
                .Where(_ => _.Superseded == null)
                //.Include(_ => _.LinePoints)
                //.ThenInclude(_ => _.Point)
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            // For en normal database uden historik ville vi bare have et Include statement efter where klausulen
            // for at få parent rækker med. Det duer imidlertid ikke for en database med historik, da parent rækken
            // kan være blevet superseeded med en nyere række.
            // Derfor genererer vi en liste af objekt ids for de line rækker, vi tager udgangspunkt i

            var lineObjectIds = lines
                .Select(_ => _.ObjectId)
                .ToList();

            var linePoints = await _context.LinePoints.AsQueryable()
                .Where(_ => _.Superseded == null && lineObjectIds.Contains(_.LineObjectId))
                .ToListAsync();
            
            var pointObjectIds = linePoints
                .Select(_ => _.PointObjectId)
                .Distinct()
                .ToList();

            var points = await _context.Points.AsQueryable()
                .Where(_ => _.Superseded == null && pointObjectIds.Contains(_.ObjectId))
                .ToListAsync();

            var pointMap = points.ToDictionary(_ => _.ObjectId, _ => _);

            var linePointGroups = linePoints.GroupBy(_ => _.LineObjectId);

            var lineDtos = lines.Select(l =>
            {
                var lineDto = new LineDto
                {
                    type = "Line",
                    id = l.ObjectId
                };

                var temp1 = linePointGroups
                    .Single(_ => _.Key == l.ObjectId)
                    .OrderBy(_ => _.SequenceQuantity)
                    .Select(x => pointMap[x.PointObjectId]);

                var temp2 = temp1
                    .Select(_ => _mapper.Map(_, _.GetType(), typeof(PointDto)) as PointDto);

                lineDto.LinePoints = temp2.ToList();

                return lineDto;
            });

            return Result<PagedList<LineDto>>.Success(
                new PagedList<LineDto>(lineDtos, count, request.Params.PageNumber, request.Params.PageSize));
        }
    }
}