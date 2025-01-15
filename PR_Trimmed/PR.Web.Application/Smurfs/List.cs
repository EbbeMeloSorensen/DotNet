using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using PR.Persistence;
using PR.Web.Application.Core;
using PR.Web.Application.Interfaces;
using PR.Web.Persistence;
using PR.Domain.Entities.Smurfs;

namespace PR.Web.Application.Smurfs
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<SmurfDto>>>
        {
            public SmurfParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<SmurfDto>>>
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
                _unitOfWorkFactory = unitOfWorkFactory;
            }

            public async Task<Result<PagedList<SmurfDto>>> Handle(
                Query request, 
                CancellationToken cancellationToken)
            {
                using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
                var predicates = new List<Expression<Func<Smurf, bool>>>();

                if (!string.IsNullOrEmpty(request.Params.Name))
                {
                    var filter = request.Params.Name.ToLower();

                    predicates.Add(x =>
                        x.Name.ToLower().Contains(filter));
                }

                var smurfs = await unitOfWork.Smurfs.Find(predicates);

                var result = _mapper.Map<IEnumerable<SmurfDto>>(smurfs);

                return Result<PagedList<SmurfDto>>.Success(
                    await PagedList<SmurfDto>.Create(result, request.Params.PageNumber,
                        request.Params.PageSize)
                );
            }
        }
    }
}
