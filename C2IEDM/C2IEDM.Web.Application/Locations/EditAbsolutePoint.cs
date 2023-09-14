using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Locations;

public class EditAbsolutePoint
{
    public class Command : IRequest<Result<Unit>>
    {
        public Domain.Entities.Geometry.Locations.Points.AbsolutePoint AbsolutePoint { get; set; }
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
            var query = _context.AbsolutePoints
                .Where(_ => _.ObjectId == request.AbsolutePoint.Id && _.Superseded == null)
                .AsQueryable();

            var absolutePoint = await query.FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            absolutePoint.Superseded = now;

            var newAbsolutePoint = new Domain.Entities.Geometry.Locations.Points.AbsolutePoint(
                request.AbsolutePoint.Id,
                now)
            {
                LatitudeCoordinate = request.AbsolutePoint.LatitudeCoordinate,
                LongitudeCoordinate = request.AbsolutePoint.LongitudeCoordinate
            };

            await _context.AbsolutePoints.AddAsync(newAbsolutePoint);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to update absolute point");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}