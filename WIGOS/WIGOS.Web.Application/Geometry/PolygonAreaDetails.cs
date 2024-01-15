using WIGOS.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry.DTOs;

namespace WIGOS.Web.Application.Geometry
{
    public class PolygonAreaDetails
    {
        public class Query : IRequest<Result<PolygonAreaDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PolygonAreaDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<PolygonAreaDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var polygonArea = await _context.PolygonAreas
                    .Include(_ => _.BoundingLine)
                    .ThenInclude(_ => _.LinePoints)
                    .ThenInclude(_ => _.Point)
                    .FirstAsync(_ => _.Id == request.Id);

                var polygonAreaDto = new PolygonAreaDto()
                {
                    id = polygonArea.Id,
                    BoundingLinePoints = polygonArea.BoundingLine.LinePoints
                        .OrderBy(_ => _.SequenceQuantity)
                        .Select(_ => _.Point.AsLocationDto() as PointDto)
                        .ToList()
                };

                return Result<PolygonAreaDto>.Success(polygonAreaDto);
            }
        }
    }
}