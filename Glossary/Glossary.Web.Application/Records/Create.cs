using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using Glossary.Domain.Entities;
using Glossary.Web.Application.Core;
using Glossary.Web.Application.Interfaces;
using Glossary.Web.Persistence;

namespace Glossary.Web.Application.Records;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public Record Record { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Record).SetValidator(new RecordValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public Handler(DataContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                x => x.UserName == _userAccessor.GetUsername());

            _context.Records.Add(request.Record);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to create record");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}