namespace MetaDataInspector.Domain.SMS;

public class StationInformation
{
    private static readonly Dictionary<int, string> _countryMap = new()
    {
        { 0, "Danmark" },
        { 1, "Grønland" },
        { 2, "Færøerne" }
    };

    private static readonly Dictionary<int, string> _stationTypeMap = new()
    {
        { 0, "Synop" },
        { 1, "Strømstation" },
        { 2, "SVK gprs" },
        { 3, "Vandstandsstation" },
        { 4, "GIWS" },
        { 5, "Pluvio" },
        { 6, "SHIP AWS" },
        { 7, "Temp ship" },
        { 8, "Lynpejlestation" },
        { 9, "Radar" },
        { 10, "Radiosonde" },
        { 11, "Historisk stationstype" },
        { 12, "Manuel Nedbør" },
        { 13, "Bølgestation" },
        { 14, "Snestation" }
    };

    private static readonly Dictionary<int, string> _statusMap = new()
    {
        { 0, "inactive" },
        { 1, "active" }
    };

    private static readonly Dictionary<int, string> _stationOwnerMap = new()
    {
        { 0, "DMI" },
        { 1, "SVK" },
        { 2, "Havne Kommuner mm" },
        { 3, "GC-net" },
        { 4, "Danske lufthavne" },
        { 5, "MITT/FRL lufthavne" },
        { 6, "Vejdirektoratet" },
        { 7, "Synop - Aarhus Uni" },
        { 8, "Asiaq" },
        { 9, "Kystdirektoratet" },
        { 10, "PROMICE" },
        { 11, "Forsvaret" }
    };

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

    public string ObjectIDAsString => $"{objectid}";
    public string StationNameAsString => stationname != null ? $"{stationname}" : "<null>";
    public string StationIDDMIAsString => stationid_dmi.HasValue ? $"{stationid_dmi.Value}" : "<null>";
    public string StationTypeAsString => stationtype.HasValue ? $"{_stationTypeMap[stationtype.Value]}" : "<null>";
    public string AccessAddressAsString => accessaddress != null ? $"{accessaddress}" : "<null>";
    public string CountryasString => country.HasValue ? $"{_countryMap[country.Value]}" : "<null>";
    public string StatusAsString => status.HasValue? $"{_statusMap[status.Value]}" : "<null>";
    public string DateFromAsString => datefrom.HasValue ? $"{datefrom.Value.AsDateTimeString(false)}" : "<null>";
    public string DateToAsString => dateto.HasValue ? $"{dateto.Value.AsDateTimeString(false)}" : "<null>";
    public string StationOwnerAsString => stationowner.HasValue ? $"{_stationOwnerMap[stationowner.Value]}" : "<null>";
    public string StationIDICAOAsString => stationid_icao != null ? stationid_icao : "<null>";
    public string HHAAsString => hha != null ? $"{hha}" : "<null>";
    public string WGSLatAsString => wgs_lat != null ? $"{wgs_lat}" : "<null>";
    public string WGSLongAsString => wgs_long != null ? $"{wgs_long}" : "<null>";
}