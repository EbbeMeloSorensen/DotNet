using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities.PR;
using PR.Persistence;
using PR.Persistence.Versioned;
using PR.Web.Application.Core;
using PR.Web.Application.Interfaces;
using PR.Web.Persistence;

namespace PR.Web.Application.People;

public class Create
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
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public Handler(
            DataContext context, 
            IUserAccessor userAccessor,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _context = context;
            _userAccessor = userAccessor;
            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
        }

        public async Task<Result<Unit>> Handle(
            Command request, 
            CancellationToken cancellationToken)
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                // HUSK AWAIT HER, ELLERS VIRKER DET IKKE!!
                await unitOfWork.People.Add(request.Person);
                unitOfWork.Complete();
            }

            return Result<Unit>.Success(Unit.Value);

            // Old
            var user = await _context.Users.FirstOrDefaultAsync(
            x => x.UserName == _userAccessor.GetUsername());

            _context.People.Add(request.Person);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to create person");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}