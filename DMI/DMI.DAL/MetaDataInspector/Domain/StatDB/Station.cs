namespace MetaDataInspector.Domain.StatDB;

public class Station
{
    public int? statid { get; set; }
    public string? icao_id { get; set; }
    public string? country { get; set; }
    public string? source { get; set; }

    public string CountryAsString => country != null ? country : "<null>";
}