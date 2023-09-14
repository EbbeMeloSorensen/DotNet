using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;

namespace C2IEDM.Web.Application.Locations.VerticalDistance;

public class List
{
    public class Query : IRequest<Result<PagedList<VerticalDistanceDto>>>
    {
        public PagingParams Params { get; set; }
        public DateTime? TimeOfInterest { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<VerticalDistanceDto>>>
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

        public async Task<Result<PagedList<VerticalDistanceDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            //var timeOfInterest = new DateTime(2024, 9, 14, 14, 21, 55, DateTimeKind.Utc);

            var dbSet = _context.VerticalDistances;

            var temp = request.TimeOfInterest.HasValue
                ? dbSet.Where(_ => _.Created < request.TimeOfInterest.Value && _.Superseded > request.TimeOfInterest.Value)
                : dbSet.Where(_ => _.Superseded == DateTime.MaxValue);

            var query = temp.ProjectTo<VerticalDistanceDto>(_mapper.ConfigurationProvider,
                    new { currentUsername = _userAccessor.GetUsername() })
                .AsQueryable();

            return Result<PagedList<VerticalDistanceDto>>.Success(
                await PagedList<VerticalDistanceDto>.CreateAsync(query, request.Params.PageNumber,
                    request.Params.PageSize)
            );
        }
    }
}