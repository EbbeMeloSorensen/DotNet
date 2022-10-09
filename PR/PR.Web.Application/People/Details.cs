using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PR.Web.Application.Core;
using PR.Web.Application.Interfaces;
using PR.Web.Persistence;

namespace PR.Web.Application.People;

public class Details
{
    public class Query : IRequest<Result<PersonDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PersonDto>>
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

        public async Task<Result<PersonDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var person = await _context.People
                .ProjectTo<PersonDto>(_mapper.ConfigurationProvider,
                    new { currentUsername = _userAccessor.GetUsername() })
                .FirstOrDefaultAsync(x => x.Id == request.Id);


            return Result<PersonDto>.Success(person);
        }
    }
}