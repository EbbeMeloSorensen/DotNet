using AutoMapper;
using AutoMapper.QueryableExtensions;
using Glossary.Web.Application.Core;
using Glossary.Web.Application.Interfaces;
using Glossary.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Glossary.Web.Application.Records;

public class Details
{
    public class Query : IRequest<Result<RecordDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<RecordDto>>
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

        public async Task<Result<RecordDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var record = await _context.Records
                .ProjectTo<RecordDto>(_mapper.ConfigurationProvider,
                    new { currentUsername = _userAccessor.GetUsername() })
                .FirstOrDefaultAsync(x => x.Id == request.Id);


            return Result<RecordDto>.Success(record);
        }
    }
}