using AutoMapper;
using AutoMapper.QueryableExtensions;
using Glossary.Web.Application.Core;
using Glossary.Web.Application.Interfaces;
using Glossary.Web.Persistence;
using MediatR;

namespace Glossary.Web.Application.Records;

public class List
{
    public class Query : IRequest<Result<PagedList<RecordDto>>>
    {
        public RecordParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<RecordDto>>>
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

        public async Task<Result<PagedList<RecordDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<RecordDto> query;

            switch (request.Params.Sorting)
            {
                case "name":
                    query = _context.Records
                        .OrderBy(d => d.Term)
                        .ProjectTo<RecordDto>(_mapper.ConfigurationProvider,
                            new { currentUsername = _userAccessor.GetUsername() })
                        .AsQueryable();
                    break;
                case "created":
                    query = _context.Records
                        .OrderByDescending(p => p.Created)
                        .ProjectTo<RecordDto>(_mapper.ConfigurationProvider,
                            new { currentUsername = _userAccessor.GetUsername() })
                        .AsQueryable();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (!string.IsNullOrEmpty(request.Params.Term))
            {
                var filter = request.Params.Term.ToLower();
                query = query.Where(x =>
                    x.Term.ToLower().Contains(filter));
            }

            if (!string.IsNullOrEmpty(request.Params.Category))
            {
                var filter = request.Params.Category.ToLower();
                query = query.Where(x =>
                    !string.IsNullOrEmpty(x.Category) && x.Category.ToLower().Contains(filter));
            }

            return Result<PagedList<RecordDto>>.Success(
                await PagedList<RecordDto>.CreateAsync(query, request.Params.PageNumber,
                    request.Params.PageSize)
            );
        }

        private List<bool> ConvertToBoolList(IEnumerable<string> items)
        {
            var result = new List<bool>();

            if (items.Contains("true"))
            {
                result.Add(true);
            }

            if (items.Contains("false"))
            {
                result.Add(false);
            }

            return result;
        }
    }
}