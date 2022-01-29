using System.Collections.Generic;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.ViewModel
{
    // Contains stuff that is used e.g in StationInformationDetailsViewModel and in CreateStationInformationDialogViewModel
    public static class SharedData
    {
        public static readonly string LoggedInUser = "ebs@dmi.dk";

        public static readonly Dictionary<StationType, string> StationTypeDisplayTextMap = new Dictionary<StationType, string>
        {
            { StationType.Synop, "Synop" },
            { StationType.Strømstation, "Strømstation" },
            { StationType.SVK_gprs, "SVK_gprs" },
            { StationType.Vandstandsstation, "Vandstandsstation" },
            { StationType.GIWS, "GIWS" },
            { StationType.Pluvio, "Pluvio" },
            { StationType.SHIP_AWS, "SHIP_AWS" },
            { StationType.Temp_ship, "Temp_ship" },
            { StationType.Lynpejlestation, "Lynpejlestation" },
            { StationType.Radar, "Radar" },
            { StationType.Radiosonde, "Radiosonde" },
            { StationType.Historisk_stationstype, "Historisk_stationstype" },
            { StationType.Manuel_nedbør, "Manuel_nedbør" },
            { StationType.Bølgestation, "Bølgestation" },
            { StationType.Snestation, "Snestation" }
        };

        public static readonly Dictionary<Status, string> StatusDisplayTextMap = new Dictionary<Status, string>
        {
            { Status.Active, "Active" },
            { Status.Inactive, "Discontinued" }
        };

        public static readonly Dictionary<Country, string> CountryDisplayTextMap = new Dictionary<Country, string>
        {
            { Country.Denmark, "Denmark" },
            { Country.Greenland, "Greenland" },
            { Country.FaroeIslands, "Faroe Islands" }
        };

        public static readonly Dictionary<StationOwner, string> StationOwnerDisplayTextMap = new Dictionary<StationOwner, string>
        {
            { StationOwner.DMI, "DMI" },
            { StationOwner.SVK, "SVK" },
            { StationOwner.Havne_Kommuner_mv, "Havne, kommuner mv" },
            { StationOwner.GC_net_Greenland_Climate_data, "GC_net_Greenland_Climate_data" },
            { StationOwner.Danske_lufthavne, "Danske lufthavne" },
            { StationOwner.MITT_GRL_lufthavne, "MITT_GRL_lufthavne" },
            { StationOwner.Vejdirektoratet, "Vejdirektoratet" },
            { StationOwner.Synop_Aarhus_Uni, "Synop_Aarhus_Uni" },
            { StationOwner.Asiaq, "Asiaq" },
            { StationOwner.Kystdirektoratet, "Kystdirektoratet" },
            { StationOwner.PROMICE_GEUS_PROMICE_net_i_Grønland, "PROMICE_GEUS_PROMICE_net_i_Grønland" },
            { StationOwner.Forsvaret, "Forsvaret" }
        };

    }
}
