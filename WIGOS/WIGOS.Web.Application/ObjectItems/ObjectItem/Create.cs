using WIGOS.Web.Persistence;
using FluentValidation;
using MediatR;
using WIGOS.Web.Application.Core;

namespace WIGOS.Web.Application.ObjectItems.ObjectItem
{
    public class Create
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
                var newObjectItem = new Domain.Entities.ObjectItems.ObjectItem(
                    Guid.NewGuid(),
                    DateTime.UtcNow)
                {
                    Name = request.ObjectItem.Name,
                    AlternativeIdentificationText = request.ObjectItem.AlternativeIdentificationText,
                };

                _context.ObjectItems.Add(newObjectItem);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<MediatR.Unit>.Failure("Failed to create object item");

                return Result<MediatR.Unit>.Success(MediatR.Unit.Value);
            }
        }
    }
}