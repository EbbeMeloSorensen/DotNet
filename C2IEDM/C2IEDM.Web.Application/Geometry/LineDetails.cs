using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Geometry.DTOs;
using C2IEDM.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Geometry;

public class LineDetails
{
    public class Query : IRequest<Result<LineDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<LineDto>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<LineDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var line = await _context.Lines
                .Include(l => l.LinePoints)
                .ThenInclude(lp => lp.Point)
                .FirstAsync(e => e.Id == request.Id);

            var lineDto = new LineDto
            {
                id = line.Id,
                LinePoints = line.LinePoints
                    .OrderBy(lp => lp.SequenceQuantity)
                    .Select(lp => lp.Point.AsLocationDto() as PointDto)
                    .ToList()
            };

            return Result<LineDto>.Success(lineDto);
        }
    }
}