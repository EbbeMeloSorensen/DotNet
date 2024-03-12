using System.Text;

namespace MigrationScriptGenerator;

public class TimeInterval
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder("[");
        sb.Append(Start.AsShortDateString());
        sb.Append(" -> ");
        sb.Append(End.AsShortDateString());
        sb.Append("]");
        return sb.ToString();
    }
}