namespace Glossary.Web.Application.Records
{
    public class RecordDto
    {
        public Guid Id { get; set; }
        public string Term { get; set; } = null!;
        public string? Source { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public DateTime? Created { get; set; }
    }
}
