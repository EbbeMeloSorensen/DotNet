using Microsoft.EntityFrameworkCore;
using MediatR;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Persistence;

namespace C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
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
            var query = _context.AbstractEnvironmentalMonitoringFacilities
                .Where(_ => _.ObjectId == request.Id && _.Superseded == DateTime.MaxValue)
                .AsQueryable();

            var abstractEnvironmentalMonitoringFacility = await query.FirstOrDefaultAsync();

            abstractEnvironmentalMonitoringFacility.Superseded = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to delete abstract environmental monitoring facility");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}