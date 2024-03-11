using System.Text;

namespace MigrationScriptGenerator;

public class TimeInterval
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder($"  Time interval in payload: ");
        sb.Append(Start.AsShortDateString());
        sb.Append(" -> ");
        sb.Append(End.AsShortDateString());
        return sb.ToString();
    }
}