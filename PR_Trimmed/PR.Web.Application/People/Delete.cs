using MediatR;
using PR.Persistence;
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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public Handler(
            DataContext context,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<Result<Unit>> Handle(
            Command request, 
            CancellationToken cancellationToken)
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var person = await unitOfWork.People.Get(request.Id);
                await unitOfWork.People.Remove(person);
                unitOfWork.Complete();
            }

            return Result<Unit>.Success(Unit.Value);

            // Old
            var personOld = await _context.People.FindAsync(request.Id);

            _context.Remove(personOld);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to delete the person");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}