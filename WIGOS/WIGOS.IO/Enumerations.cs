namespace WIGOS.IO
{
    public enum StationType
    {
        Synop,
        Strømstation,
        SVK_gprs,
        Vandstandsstation,
        GIWS,
        Pluvio,
        SHIP_AWS,
        Temp_ship,
        Lynpejlestation,
        Radar,
        Radiosonde,
        Historisk_stationstype,
        Manuel_nedbør,
        Bølgestation,
        Snestation
    }

    public enum StationOwner
    {
        DMI,
        SVK,
        Havne_Kommuner_mv,
        GC_net_Greenland_Climate_data,
        Danske_lufthavne,
        MITT_GRL_lufthavne,
        Vejdirektoratet,
        Synop_Aarhus_Uni,
        Asiaq,
        Kystdirektoratet,
        PROMICE_GEUS_PROMICE_net_i_Grønland,
        Forsvaret
    }

    public enum Country
    {
        Denmark,
        Greenland,
        FaroeIslands
    }

    public enum Status
    {
        Inactive,
        Active
    }

    public enum SensorType
    {
        Vindsensor,
        Vindsensor2,
        Temperatur_Fugt,
        Temperatur_Fugt2,
        Nedbørmåler,
        Barometer,
        Barometer2,
        Græstermometer,
        Jordtermometer_10_cm,
        Jordtermometer_30_cm,
        Bladfugtmåler,
        Globalstråling,
        PWS,
        Ceilometer,
        CTD_STD,
        Radar_Vandstand_,
        Sonar,
        Trykmåler,
        Boblemåler,
        Radiosonde,
        ADCP,
        Lynpejler,
        Vandtemperatur
    }
}
