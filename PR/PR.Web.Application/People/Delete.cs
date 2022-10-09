using MediatR;
using PR.Web.Application.Core;
using PR.Web.Persistence;

namespace PR.Web.Application.People;

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
            var person = await _context.People.FindAsync(request.Id);

            _context.Remove(person);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to delete the person");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}