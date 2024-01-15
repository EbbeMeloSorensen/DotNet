using WIGOS.Web.Persistence;
using FluentValidation;
using MediatR;
using WIGOS.Web.Application.Core;

namespace WIGOS.Web.Application.Geometry.VerticalDistance
{
    public class Create
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
                var newVerticalDistance = new Domain.Entities.Geometry.VerticalDistance(
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
}