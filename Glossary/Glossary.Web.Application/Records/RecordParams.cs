using Glossary.Web.Application.Core;

namespace Glossary.Web.Application.Records
{
    public class RecordParams : PagingParams
    {
        public string? Term { get; set; }
        public string? Category { get; set; }
        public string Sorting { get; set; }

        public RecordParams()
        {
            Sorting = "name";
        }
    }
}
