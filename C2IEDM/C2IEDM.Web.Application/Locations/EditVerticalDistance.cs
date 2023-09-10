using AutoMapper;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.People;
using C2IEDM.Web.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Locations;

public class EditVerticalDistance
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
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var query = _context.VerticalDistances
                .Where(_ => _.ObjectId == request.VerticalDistance.Id && _.Superseded == null)
                .AsQueryable();

            var verticalDistance = await query.FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            verticalDistance.Superseded = now;

            var newVerticalDistance = new VerticalDistance(
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