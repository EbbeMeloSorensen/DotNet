using System.Text;

namespace MigrationScriptGenerator;

public class TimeInterval
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public TimeSpan Duration { get => End - Start; }

    public override string ToString()
    {
        var sb = new StringBuilder("[");
        sb.Append(Start.AsDateTimeString());
        sb.Append(" -> ");
        sb.Append(End.AsDateTimeString());
        sb.Append("]");
        return sb.ToString();
    }
}