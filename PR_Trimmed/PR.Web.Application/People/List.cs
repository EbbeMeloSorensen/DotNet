using System.Globalization;
using AutoMapper;
using MediatR;
using PR.Persistence;
using PR.Persistence.Versioned;
using PR.Web.Persistence;
using PR.Web.Application.Core;
using PR.Web.Application.Interfaces;
using System.Linq.Expressions;
using PR.Domain.Entities.PR;

namespace PR.Web.Application.People;

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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public Handler(
            DataContext context, 
            IMapper mapper, 
            IUserAccessor userAccessor,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _context = context;
            _mapper = mapper;
            _userAccessor = userAccessor;
            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
        }

        public async Task<Result<PagedList<PersonDto>>> Handle(
            Query request, 
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Params.HistoricalTime))
            {
                try
                {
                    var dbTime = DateTime.ParseExact(request.Params.HistoricalTime, "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                    (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.HistoricalTime = dbTime;
                }
                catch (Exception e)
                {
                    return Result<PagedList<PersonDto>>.Failure("Invalid time format");
                }
            }

            if (request.Params.IncludeHistoricalObjects.HasValue)
            {
                (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.IncludeHistoricalObjects = request.Params.IncludeHistoricalObjects.Value;
            }

            if (!string.IsNullOrEmpty(request.Params.DatabaseTime))
            {
                try
                {
                    var dbTime = DateTime.ParseExact(request.Params.DatabaseTime, "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                    (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = dbTime;
                }
                catch (Exception e)
                {
                    return Result<PagedList<PersonDto>>.Failure("Invalid time format");
                }
            }

            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var predicates = new List<Expression<Func<Person, bool>>>();

            if (!string.IsNullOrEmpty(request.Params.Name))
            {
                var filter = request.Params.Name.ToLower();

                predicates.Add(x =>
                    x.FirstName.ToLower().Contains(filter) ||
                    (!string.IsNullOrEmpty(x.Surname) && x.Surname.ToLower().Contains(filter)));
            }

            var people = await unitOfWork.People.Find(predicates);

            var result = _mapper.Map<IEnumerable<PersonDto>>(people);

            return Result<PagedList<PersonDto>>.Success(
                await PagedList<PersonDto>.Create(result, request.Params.PageNumber,
                    request.Params.PageSize)
            );

            /*
            // Old
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
            */
        }

        private List<bool> ConvertToBoolList(
            IEnumerable<string> items)
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