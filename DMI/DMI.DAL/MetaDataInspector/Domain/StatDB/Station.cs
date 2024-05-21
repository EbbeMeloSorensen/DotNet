namespace MetaDataInspector.Domain.StatDB;

public class Station
{
    public int? statid { get; set; }
    public string? icao_id { get; set; }
    public string? country { get; set; }
    public string? source { get; set; }

    public string StatIDAsString => statid.HasValue ? $"{statid.Value}" : "<null>";
    public string IcaoIDAsString => !string.IsNullOrEmpty(icao_id) ? icao_id : "<null>";
    public string CountryAsString => !string.IsNullOrEmpty(country) ? country : "<null>";
    public string SourceAsString => !string.IsNullOrEmpty(source) ? source : "<null>";
}