namespace MetaDataInspector.Domain.StatDB;

public class Position
{
    public int? statid { get; set; }
    public string? entity { get; set; }
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public double? lat { get; set; }
    public double? @long { get; set; }
    public double? height { get; set; }
}