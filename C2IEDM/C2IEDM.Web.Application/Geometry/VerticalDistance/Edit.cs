using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Geometry.VerticalDistance;

public class Edit
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

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var query = _context.VerticalDistances
                .Where(_ => _.ObjectId == request.VerticalDistance.Id && _.Superseded == DateTime.MaxValue)
                .AsQueryable();

            var verticalDistance = await query.FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            verticalDistance.Superseded = now;

            var newVerticalDistance = new Domain.Entities.Geometry.VerticalDistance(
                request.VerticalDistance.Id,
                now)
            {
                Dimension = request.VerticalDistance.Dimension
            };

            await _context.VerticalDistances.AddAsync(newVerticalDistance);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to update vertical distance");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}