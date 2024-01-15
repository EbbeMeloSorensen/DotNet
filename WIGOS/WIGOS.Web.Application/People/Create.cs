using WIGOS.Domain.Entities;
using WIGOS.Web.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Interfaces;

namespace WIGOS.Web.Application.People
{
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

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(
                    x => x.UserName == _userAccessor.GetUsername());

                _context.People.Add(request.Person);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create person");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
