using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.Locations;

public class EllipseDetails
{
    public class Query : IRequest<Result<EllipseDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<EllipseDto>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<EllipseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ellipse = await _context.Ellipses
                .Include(_ => _.CentrePoint)
                .Include(_ => _.FirstConjugateDiameterPoint)
                .Include(_ => _.SecondConjugateDiameterPoint)
                .FirstAsync(e => e.Id == request.Id);

            var ellipseDto = new EllipseDto
            {
                id = ellipse.Id,
                CentrePoint = ellipse.CentrePoint.AsLocationDto() as PointDto,
                FirstConjugateDiameterPoint = ellipse.FirstConjugateDiameterPoint.AsLocationDto() as PointDto,
                SecondConjugateDiameterPoint = ellipse.SecondConjugateDiameterPoint.AsLocationDto() as PointDto
            };

            return Result<EllipseDto>.Success(ellipseDto);
        }
    }
}