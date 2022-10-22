using AutoMapper;
using FluentValidation;
using Glossary.Domain.Entities;
using Glossary.Web.Application.Core;
using Glossary.Web.Persistence;
using MediatR;

namespace Glossary.Web.Application.Records;

public class Edit
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
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var record = await _context.Records.FindAsync(request.Record.Id);

            if (record == null) return null;

            // This is a smart way of mapping, so we avoid having to maintain our own mappers
            _mapper.Map(request.Record, record);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to update record");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}