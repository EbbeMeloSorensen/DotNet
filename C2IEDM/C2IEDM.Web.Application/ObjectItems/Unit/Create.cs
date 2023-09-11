using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Persistence;
using FluentValidation;
using MediatR;

namespace C2IEDM.Web.Application.ObjectItems.Unit;

public class Create
{
    public class Command : IRequest<Result<MediatR.Unit>>
    {
        public Domain.Entities.ObjectItems.Organisations.Unit Unit { get; set; }
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

        public async Task<Result<MediatR.Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var newUnit = new Domain.Entities.ObjectItems.Organisations.Unit(
                Guid.NewGuid(),
                DateTime.UtcNow)
            {
                Name = request.Unit.Name,
                AlternativeIdentificationText = request.Unit.AlternativeIdentificationText,
                NickName = request.Unit.NickName,
                FormalAbbreviatedName = request.Unit.FormalAbbreviatedName
            };

            _context.Units.Add(newUnit);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<MediatR.Unit>.Failure("Failed to create unit");

            return Result<MediatR.Unit>.Success(MediatR.Unit.Value);
        }
    }
}