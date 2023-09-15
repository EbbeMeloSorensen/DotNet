using MediatR;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.WIGOS.DTOs;
using C2IEDM.Web.Application.ObjectItems.DTOs;
using AutoMapper;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.ObjectItems;
using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.ObjectItems;

namespace C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacility;

public enum AbstractEnvironmentalMonitoringFacilityCategory
{
    AbstractEnvironmentalMonitoringFacility,
    ObservingFacility
}

public class List
{
    public class Query : IRequest<Result<PagedList<AbstractEnvironmentalMonitoringFacilityDto>>>
    {
        public AbstractEnvironmentalMonitoringFacilityCategory Category { get; set; }
        public PagingParams Params { get; set; }
        public DateTime? TimeOfInterest { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<AbstractEnvironmentalMonitoringFacilityDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<AbstractEnvironmentalMonitoringFacilityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = request.Category switch
            {
                AbstractEnvironmentalMonitoringFacilityCategory.ObservingFacility => _context.ObservingFacilities.AsQueryable(),
                AbstractEnvironmentalMonitoringFacilityCategory.AbstractEnvironmentalMonitoringFacility => _context.AbstractEnvironmentalMonitoringFacilities.AsQueryable(),
                _ => null
            };

            query = request.TimeOfInterest.HasValue
                ? query.Where(_ => _.Created < request.TimeOfInterest.Value && _.Superseded > request.TimeOfInterest.Value)
                : query.Where(_ => _.Superseded == DateTime.MaxValue);

            var count = await query.CountAsync();

            var abstractEnvironmentalMonitoringFacilities = await query
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
            .ToListAsync();

            var abstractEnvironmentalMonitoringFacilityDtos = abstractEnvironmentalMonitoringFacilities.Select(_ =>
            {
                var temp = _mapper.Map(_, _.GetType(), typeof(AbstractEnvironmentalMonitoringFacilityDto));

                if (temp is not AbstractEnvironmentalMonitoringFacilityDto abstractEnvironmentalMonitoringFacilityDto)
                {
                    throw new InvalidDataException();
                }

                return abstractEnvironmentalMonitoringFacilityDto;
            });

            return Result<PagedList<AbstractEnvironmentalMonitoringFacilityDto>>.Success(
                new PagedList<AbstractEnvironmentalMonitoringFacilityDto>(abstractEnvironmentalMonitoringFacilityDtos, count, request.Params.PageNumber, request.Params.PageSize));
        }
    }
}