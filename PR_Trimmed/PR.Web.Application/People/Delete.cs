﻿using MediatR;
using PR.Persistence;
using PR.Persistence.Versioned;
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
            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
        }

        public async Task<Result<Unit>> Handle(
            Command request, 
            CancellationToken cancellationToken)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    var person = await unitOfWork.People.Get(request.Id);
                    await unitOfWork.People.Remove(person);
                    unitOfWork.Complete();
                }
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure($"Error deleting person: {e.Message}");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}