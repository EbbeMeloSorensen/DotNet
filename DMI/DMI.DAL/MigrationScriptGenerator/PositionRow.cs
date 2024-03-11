namespace MigrationScriptGenerator;

public class PositionRow
{
    public int StationIdDMI { get; set; }

    public string DummyStationString { get; set; }
    
    public string StartString { get; set; }
    
    public string EndString { get; set; }
    
    public string LatitudeString { get; set; }
    
    public string LongitudeString { get; set; }
    
    public string HeightString { get; set; }

    public DateTime StartTime => StartString.Trim('\'').AsDateTimeUTC();

    public DateTime EndTime => EndString.Trim('\'').AsDateTimeUTC();
}