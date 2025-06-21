using MediatR;
using PR.Web.Application.Core;
using PR.Web.Application.Landing;
using PR.Web.Application.People;

public class Get
{
    public class Query : IRequest<Result<LandingDto>>
    {
    }

    public class Handler : IRequestHandler<Query, Result<LandingDto>>
    {
        public async Task<Result<LandingDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var result = new LandingDto
            {
                Bamse = "Bamse",
                Kylling = "Kylling"
            };

            return Result<LandingDto>.Success(result);
        }
    }
}