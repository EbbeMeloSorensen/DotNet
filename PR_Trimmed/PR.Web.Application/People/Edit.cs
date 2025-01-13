using AutoMapper;
using FluentValidation;
using MediatR;
using PR.Domain;
using PR.Domain.Entities.PR;
using PR.Persistence;
using PR.Persistence.Versioned;
using PR.Web.Application.Core;
using PR.Web.Persistence;

namespace PR.Web.Application.People;

public class Edit
{
    public class Command : IRequest<Result<Unit>>
    {
        public Person Person { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Person).SetValidator(new PersonValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public Handler(
            DataContext context, 
            IMapper mapper,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
        }

        public async Task<Result<Unit>> Handle(
            Command request, 
            CancellationToken cancellationToken)
        {
            try
            {
                using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
                await unitOfWork.People.Update(request.Person);
                unitOfWork.Complete();
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure($"Error editing person: {e.Message}");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}