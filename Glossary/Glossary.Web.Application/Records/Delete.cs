using Glossary.Web.Application.Core;
using Glossary.Web.Persistence;
using MediatR;

namespace Glossary.Web.Application.Records;

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
            var record = await _context.Records.FindAsync(request.Id);

            _context.Remove(record);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to delete the record");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}