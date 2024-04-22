namespace MigrationScriptGenerator;

public class LeeIndexRow
{
    public int StationIdDMI { get; set; }

    public string? StartString { get; set; }
    
    public string? EndString { get; set; }

    public double? S { get; set; }

    public double? SW { get; set; }

    public double? W { get; set; }

    public double? NW { get; set; }

    public double? N { get; set; }

    public double? NE { get; set; }

    public double? E { get; set; }

    public double? SE { get; set; }

    public double? Index { get; set; }

    public string? Comment { get; set; }

    public DateTime StartTime
    {
        get
        {
            return StartString == null
            ? new DateTime() 
            :  StartString.Trim('\'').AsDateTimeUTC();
        }
    } 

    public DateTime EndTime
    {
        get
        {
            return EndString == null
            ? new DateTime() 
            :  EndString.Trim('\'').AsDateTimeUTC();
        }
    } 
}
