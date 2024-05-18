namespace MetaDataInspector.Domain.SMS;

public class StationInformation
{
    public int objectid { get; set; }
    public string? stationname { get; set; }
    public int? stationid_dmi { get; set; }
    public int? stationtype { get; set; }
    public string? accessaddress { get; set; }
    public int? country { get; set; }
    public int? status { get; set; }
    public DateTime? datefrom { get; set; }
    public DateTime? dateto { get; set; }
    public int? stationowner { get; set; }
    public string? stationid_icao { get; set; }
    public double? hha { get; set; }
    public double? wgs_lat { get; set; }
    public double? wgs_long { get; set; }
}