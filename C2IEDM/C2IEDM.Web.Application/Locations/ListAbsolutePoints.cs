using MediatR;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Persistence;
using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry.Locations;
using AutoMapper;

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
                .Include(_ => _.VerticalDistance)
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            var absolutePointDtos = absolutePoints.Select(l =>
            {
                var temp = _mapper.Map(l, l.GetType(), typeof(AbsolutePointDto));

                if (temp is not AbsolutePointDto absolutePointDto)
                {
                    throw new InvalidDataException();
                }

                return absolutePointDto;
            });

            return Result<PagedList<AbsolutePointDto>>.Success(
                new PagedList<AbsolutePointDto>(absolutePointDtos, count, request.Params.PageNumber, request.Params.PageSize));
        }
    }
}