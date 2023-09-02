using AutoMapper;
using AutoMapper.QueryableExtensions;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Web.Persistence;
using MediatR;

namespace C2IEDM.Web.Application.People;

public class List
{
    public class Query : IRequest<Result<PagedList<PersonDto>>>
    {
        public PersonParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<PersonDto>>>
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

        public async Task<Result<PagedList<PersonDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<PersonDto> query;

            switch (request.Params.Sorting)
            {
                case "name":
                    query = _context.People
                        .OrderBy(d => d.FirstName)
                        .ThenByDescending(d => d.Surname == null)
                        .ThenBy(d => d.Surname)
                        .ProjectTo<PersonDto>(_mapper.ConfigurationProvider,
                            new { currentUsername = _userAccessor.GetUsername() })
                        .AsQueryable();
                    break;
                case "created":
                    query = _context.People
                        .OrderByDescending(p => p.Created)
                        .ProjectTo<PersonDto>(_mapper.ConfigurationProvider,
                            new { currentUsername = _userAccessor.GetUsername() })
                        .AsQueryable();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (!string.IsNullOrEmpty(request.Params.Dead))
            {
                var filterAsListOfStrings = new List<string>(request.Params.Dead.Split("|"));

                if (filterAsListOfStrings.Count == 1 && filterAsListOfStrings.Single() == "null")
                {
                    query = query.Where(x => !x.Dead.HasValue);
                }
                else
                {
                    var filter = ConvertToBoolList(filterAsListOfStrings);

                    query = filterAsListOfStrings.Contains("null")
                        ? query = query.Where(x => !x.Dead.HasValue || filter.Contains(x.Dead.Value))
                        : query = query.Where(x => x.Dead.HasValue && filter.Contains(x.Dead.Value));
                }
            }

            if (!string.IsNullOrEmpty(request.Params.Name))
            {
                var filter = request.Params.Name.ToLower();
                query = query.Where(x =>
                    x.FirstName.ToLower().Contains(filter) ||
                    (!string.IsNullOrEmpty(x.Surname) && x.Surname.ToLower().Contains(filter)));
            }

            if (!string.IsNullOrEmpty(request.Params.Category))
            {
                var filter = request.Params.Category.ToLower();
                query = query.Where(x =>
                    !string.IsNullOrEmpty(x.Category) && x.Category.ToLower().Contains(filter));
            }

            return Result<PagedList<PersonDto>>.Success(
                await PagedList<PersonDto>.CreateAsync(query, request.Params.PageNumber,
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