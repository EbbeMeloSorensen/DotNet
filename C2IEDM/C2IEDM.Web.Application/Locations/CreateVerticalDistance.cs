using C2IEDM.Domain.Entities.Geometry;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Domain.Entities.Geometry.Locations;

namespace C2IEDM.Web.Application.Locations;

public class CreateVerticalDistance
{
    public class Command : IRequest<Result<Unit>>
    {
        public VerticalDistance VerticalDistance { get; set; }
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
            var newVerticalDistance = new VerticalDistance(
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