using CommandLine;

namespace DMI.SMS.UI.Console.Verbs.SensorLocation
{
    [Verb("createSensorLocation", HelpText = "Create Sensor Location")]
    public sealed class Create
    {
        [Option('i', "station id", Required = true, HelpText = "Station Id")]
        public string StationId { get; set; }
    }
}
