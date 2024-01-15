using WIGOS.Web.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WIGOS.Web.Application.Core;

namespace WIGOS.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities.ObservingFacility
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities.ObservingFacility ObservingFacility { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<Command, Result<MediatR.Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var query = _context.ObservingFacilities
                    .Where(_ => _.ObjectId == request.ObservingFacility.Id && _.Superseded == DateTime.MaxValue)
                    .AsQueryable();

                var observingFacility = await query.FirstOrDefaultAsync();

                var now = DateTime.UtcNow;

                observingFacility.Superseded = now;

                var newObservingFacility = new Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities.ObservingFacility(
                    request.ObservingFacility.Id,
                    now)
                {
                    Name = request.ObservingFacility.Name,
                    DateEstablished = request.ObservingFacility.DateEstablished,
                    DateClosed = request.ObservingFacility.DateClosed
                };

                await _context.ObservingFacilities.AddAsync(newObservingFacility);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update observing facility");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}