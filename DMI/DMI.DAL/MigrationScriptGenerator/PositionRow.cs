namespace MigrationScriptGenerator;

public class PositionRow
{
    public int StationIdDMI { get; set; }

    public string? Entity { get; set; }
    
    public string? StartString { get; set; }
    
    public string? EndString { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public double? Height { get; set; }

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