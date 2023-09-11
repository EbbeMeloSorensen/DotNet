using MediatR;
using FluentValidation;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;

namespace C2IEDM.Web.Application.ObjectItems.Organisation;

public class Create
{
    public class Command : IRequest<Result<MediatR.Unit>>
    {
        public Domain.Entities.ObjectItems.Organisations.Organisation Organisation { get; set; }
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
            var newOrganisation = new Domain.Entities.ObjectItems.Organisations.Organisation(
                Guid.NewGuid(),
                DateTime.UtcNow)
            {
                Name = request.Organisation.Name,
                AlternativeIdentificationText = request.Organisation.AlternativeIdentificationText,
                NickName = request.Organisation.NickName
            };

            _context.Organisations.Add(newOrganisation);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<MediatR.Unit>.Failure("Failed to create organisation");

            return Result<MediatR.Unit>.Success(MediatR.Unit.Value);
        }
    }
}