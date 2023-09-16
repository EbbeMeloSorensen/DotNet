using MediatR;
using FluentValidation;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;

namespace C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities.ObservingFacility;

public class Create
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

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var newObservingFacility = new Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities.ObservingFacility(
                Guid.NewGuid(),
                DateTime.UtcNow)
            {
                Name = request.ObservingFacility.Name,
                DateEstablished = request.ObservingFacility.DateEstablished,
                DateClosed = request.ObservingFacility.DateClosed,
            };

            _context.ObservingFacilities.Add(newObservingFacility);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to create observing facility item");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}