using AutoMapper;
using AutoMapper.QueryableExtensions;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Persistence;
using MediatR;

namespace C2IEDM.Web.Application.Locations.VerticalDistance;

public class List
{
    public class Query : IRequest<Result<PagedList<VerticalDistanceDto>>>
    {
        public Params Params { get; set; }
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
            var query = _context.VerticalDistances
                .Where(_ => _.Superseded == null)
                .ProjectTo<VerticalDistanceDto>(_mapper.ConfigurationProvider,
                    new { currentUsername = _userAccessor.GetUsername() })
                .AsQueryable();

            return Result<PagedList<VerticalDistanceDto>>.Success(
                await PagedList<VerticalDistanceDto>.CreateAsync(query, request.Params.PageNumber,
                    request.Params.PageSize)
            );
        }
    }
}