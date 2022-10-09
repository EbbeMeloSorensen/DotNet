using AutoMapper;
using FluentValidation;
using MediatR;
using PR.Domain.Entities;
using PR.Web.Application.Core;
using PR.Web.Persistence;

namespace PR.Web.Application.People;

public class Edit
{
    public class Command : IRequest<Result<Unit>>
    {
        public Person Person { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Person).SetValidator(new PersonValidator());
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
            var person = await _context.People.FindAsync(request.Person.Id);

            if (person == null) return null;

            // This is a smart way of mapping, so we avoid having to maintain our own mappers
            _mapper.Map(request.Person, person);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to update person");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}