using FluentValidation;
using Glossary.Domain.Entities;

namespace Glossary.Web.Application.Records
{
    public class RecordValidator : AbstractValidator<Record>
    {
        public RecordValidator()
        {
            RuleFor(x => x.Term).NotEmpty();
        }
    }
}
