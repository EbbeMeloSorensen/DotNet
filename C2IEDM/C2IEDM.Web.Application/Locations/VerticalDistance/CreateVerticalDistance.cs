using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Web.Persistence;
using FluentValidation;
using MediatR;

namespace C2IEDM.Web.Application.Locations.VerticalDistance;

public class CreateVerticalDistance
{
    public class Command : IRequest<Result<Unit>>
    {
        public Domain.Entities.Geometry.VerticalDistance VerticalDistance { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
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
            var newVerticalDistance = new Domain.Entities.Geometry.VerticalDistance(
                Guid.NewGuid(),
                DateTime.UtcNow)
            {
                Dimension = request.VerticalDistance.Dimension
            };

            _context.VerticalDistances.Add(newVerticalDistance);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to create vertical distance");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}