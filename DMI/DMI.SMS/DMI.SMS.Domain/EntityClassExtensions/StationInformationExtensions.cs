using System.Collections.Generic;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Domain.EntityClassExtensions
{
    public static class StationInformationExtensions
    {
        public static StationInformation Clone(
            this StationInformation stationInformation)
        {
            var clone = new StationInformation();
            clone.CopyAttributes(stationInformation);
            return clone;
        }

        public static void CopyAttributes(
            this StationInformation stationInformation,
            StationInformation other)
        {
            stationInformation.GdbArchiveOid = other.GdbArchiveOid;
            stationInformation.GlobalId = other.GlobalId;
            stationInformation.ObjectId = other.ObjectId;
            stationInformation.CreatedUser = other.CreatedUser;
            stationInformation.CreatedDate = other.CreatedDate;
            stationInformation.LastEditedUser = other.LastEditedUser;
            stationInformation.LastEditedDate = other.LastEditedDate;
            stationInformation.GdbFromDate = other.GdbFromDate;
            stationInformation.GdbToDate = other.GdbToDate;
            stationInformation.StationName = other.StationName;
            stationInformation.StationIDDMI = other.StationIDDMI;
            stationInformation.Stationtype = other.Stationtype;
            stationInformation.AccessAddress = other.AccessAddress;
            stationInformation.Country = other.Country;
            stationInformation.Status = other.Status;
            stationInformation.DateFrom = other.DateFrom;
            stationInformation.DateTo = other.DateTo;
            stationInformation.StationOwner = other.StationOwner;
            stationInformation.Comment = other.Comment;
            stationInformation.Stationid_icao = other.Stationid_icao;
            stationInformation.Referencetomaintenanceagreement = other.Referencetomaintenanceagreement;
            stationInformation.Facilityid = other.Facilityid;
            stationInformation.Si_utm = other.Si_utm;
            stationInformation.Si_northing = other.Si_northing;
            stationInformation.Si_easting = other.Si_easting;
            stationInformation.Si_geo_lat = other.Si_geo_lat;
            stationInformation.Si_geo_long = other.Si_geo_long;
            stationInformation.Serviceinterval = other.Serviceinterval;
            stationInformation.Lastservicedate = other.Lastservicedate;
            stationInformation.Nextservicedate = other.Nextservicedate;
            stationInformation.Addworkforcedate = other.Addworkforcedate;
            stationInformation.Lastvisitdate = other.Lastvisitdate;
            stationInformation.Altstationid = other.Altstationid;
            stationInformation.Wmostationid = other.Wmostationid;
            stationInformation.Regionid = other.Regionid;
            stationInformation.Wigosid = other.Wigosid;
            stationInformation.Wmocountrycode = other.Wmocountrycode;
            stationInformation.Hha = other.Hha;
            stationInformation.Hhp = other.Hhp;
            stationInformation.Wmorbsn = other.Wmorbsn;
            stationInformation.Wmorbcn = other.Wmorbcn;
            stationInformation.Wmorbsnradio = other.Wmorbsnradio;
            stationInformation.Wgs_lat = other.Wgs_lat;
            stationInformation.Wgs_long = other.Wgs_long;
            stationInformation.Shape = other.Shape;
        }

        // This method determines which of the LOGICAL attributes differs between the two instances
        public static List<string> Compare(
            this StationInformation stationInformation,
            StationInformation other,
            bool onlyImportant)
        {
            var result = new List<string>();

            if (stationInformation.StationName != other.StationName) result.Add("StationName");

            if (stationInformation.StationIDDMI != other.StationIDDMI) result.Add("StationIDDMI");
            if (stationInformation.Stationtype != other.Stationtype) result.Add("Stationtype");
            if (stationInformation.Status != other.Status) result.Add("Status");
            if (stationInformation.DateFrom != other.DateFrom) result.Add("DateFrom");
            if (stationInformation.DateTo != other.DateTo) result.Add("DateTo");
            if (stationInformation.StationOwner != other.StationOwner) result.Add("StationOwner");
            if (stationInformation.Hha != other.Hha) result.Add("Hha");
            if (stationInformation.Hhp != other.Hhp) result.Add("Hhp");
            if (stationInformation.Wgs_lat != other.Wgs_lat) result.Add("Wgs_lat");
            if (stationInformation.Wgs_long != other.Wgs_long) result.Add("Wgs_long");

            if (onlyImportant)
            {
                return result;
            }

            if (stationInformation.AccessAddress != other.AccessAddress) result.Add("AccessAddress");
            if (stationInformation.Country != other.Country) result.Add("Country");
            if (stationInformation.Comment != other.Comment) result.Add("Comment");
            if (stationInformation.Stationid_icao != other.Stationid_icao) result.Add("Stationid_icao");
            if (stationInformation.Referencetomaintenanceagreement != other.Referencetomaintenanceagreement) result.Add("Referencetomaintenanceagreement");
            if (stationInformation.Facilityid != other.Facilityid) result.Add("Facilityid");
            if (stationInformation.Si_utm != other.Si_utm) result.Add("Si_utm");
            if (stationInformation.Si_northing != other.Si_northing) result.Add("Si_northing");
            if (stationInformation.Si_easting != other.Si_easting) result.Add("Si_easting");
            if (stationInformation.Si_geo_lat != other.Si_geo_lat) result.Add("Si_geo_lat");
            if (stationInformation.Si_geo_long != other.Si_geo_long) result.Add("Si_geo_long");
            if (stationInformation.Serviceinterval != other.Serviceinterval) result.Add("Serviceinterval");
            if (stationInformation.Lastservicedate != other.Lastservicedate) result.Add("Lastservicedate");
            if (stationInformation.Nextservicedate != other.Nextservicedate) result.Add("Nextservicedate");
            if (stationInformation.Addworkforcedate != other.Addworkforcedate) result.Add("Addworkforcedate");
            if (stationInformation.Lastvisitdate != other.Lastvisitdate) result.Add("Lastvisitdate");
            if (stationInformation.Altstationid != other.Altstationid) result.Add("Altstationid");
            if (stationInformation.Wmostationid != other.Wmostationid) result.Add("Wmostationid");
            if (stationInformation.Regionid != other.Regionid) result.Add("Regionid");
            if (stationInformation.Wigosid != other.Wigosid) result.Add("Wigosid");
            if (stationInformation.Wmocountrycode != other.Wmocountrycode) result.Add("Wmocountrycode");
            if (stationInformation.Wmorbsn != other.Wmorbsn) result.Add("Wmorbsn");
            if (stationInformation.Wmorbcn != other.Wmorbcn) result.Add("Wmorbcn");
            if (stationInformation.Wmorbsnradio != other.Wmorbsnradio) result.Add("Wmorbsnradio");

            return result;
        }
    }
}
