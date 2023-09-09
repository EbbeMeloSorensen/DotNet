using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Domain.Entities.Geometry.Locations;

namespace C2IEDM.Web.Application.Locations;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public Location Location { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Location).SetValidator(new LocationValidator());
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

            _context.Locations.Add(request.Location);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to create location");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}