using Newtonsoft.Json;
using System.Xml.Serialization;

namespace WIGOS.IO
{
    public class DataIOHandler
    {
        private XmlSerializer _xmlSerializer;

        private XmlSerializer XmlSerializer
        {
            get
            {
                if (_xmlSerializer != null)
                    return _xmlSerializer;

                var xOver = new XmlAttributeOverrides();
                var attrs = new XmlAttributes { XmlIgnore = true };
                xOver.Add(typeof(StationInformation), "SensorLocations", attrs);
                xOver.Add(typeof(StationInformation), "ContactPersons", attrs);
                xOver.Add(typeof(StationInformation), "LegalOwners", attrs);
                xOver.Add(typeof(StationInformation), "StationKeepers", attrs);
                xOver.Add(typeof(StationInformation), "MaintenanceRegulations", attrs);
                xOver.Add(typeof(StationInformation), "Errors", attrs);
                _xmlSerializer = new XmlSerializer(typeof(SMSData), xOver);

                return _xmlSerializer;
            }
        }

        public void ImportDataFromJson(
            string fileName,
            out IList<StationInformation> stationInformations)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                stationInformations = JsonConvert.DeserializeObject<List<StationInformation>>(json);
            }
        }
    }
}