using MediatR;
using C2IEDM.Web.Application.Core;
using FluentValidation;
using C2IEDM.Web.Persistence;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.ObjectItems.ObjectItem;

public class Edit
{
    public class Command : IRequest<Result<MediatR.Unit>>
    {
        public Domain.Entities.ObjectItems.ObjectItem ObjectItem { get; set; }
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
            var query = _context.ObjectItems
                .Where(_ => _.ObjectId == request.ObjectItem.Id && _.Superseded == null)
                .AsQueryable();

            var objectItem = await query.FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            objectItem.Superseded = now;

            var newObjectItem = new Domain.Entities.ObjectItems.ObjectItem(
                request.ObjectItem.Id,
                now)
            {
                Name = request.ObjectItem.Name
            };

            await _context.ObjectItems.AddAsync(newObjectItem);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<MediatR.Unit>.Failure("Failed to update object item");

            return Result<MediatR.Unit>.Success(MediatR.Unit.Value);
        }
    }
}