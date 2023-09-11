using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Application.ObjectItems;

public class Delete
{
    public class Command : IRequest<Result<MediatR.Unit>>
    {
        public Guid Id { get; set; }
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
                .Where(_ => _.ObjectId == request.Id && _.Superseded == null)
                .AsQueryable();

            var objectItem = await query.FirstOrDefaultAsync();

            objectItem.Superseded = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<MediatR.Unit>.Failure("Failed to delete object item");

            return Result<MediatR.Unit>.Success(MediatR.Unit.Value);
        }
    }
}