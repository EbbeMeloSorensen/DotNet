using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PR.Persistence;
using PR.Persistence.Versioned;
using PR.Web.Application.Core;
using PR.Web.Application.Interfaces;
using PR.Web.Persistence;
using System.Globalization;

namespace PR.Web.Application.People;

public class Details
{
    public class Query : IRequest<Result<PersonDto>>
    {
        public Guid Id { get; set; }
        public VersioningParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PersonDto>>
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

        public async Task<Result<PersonDto>> Handle(
            Query request, 
            CancellationToken cancellationToken)
        {
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
                    return Result<PersonDto>.Failure("Invalid time format");
                }
            }

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var person = await unitOfWork.People.Get(request.Id);
                var result = _mapper.Map<PersonDto>(person);

                return Result<PersonDto>.Success(result);
            }

            //var person = await _context.People
            //    .ProjectTo<PersonDto>(_mapper.ConfigurationProvider,
            //        new { currentUsername = _userAccessor.GetUsername() })
            //    .FirstOrDefaultAsync(x => x.Id == request.Id);


            //return Result<PersonDto>.Success(person);
        }
    }
}