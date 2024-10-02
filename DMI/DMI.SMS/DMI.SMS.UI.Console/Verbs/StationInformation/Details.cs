using CommandLine;

namespace DMI.SMS.UI.Console.Verbs.StationInformation;

[Verb("stationInformationDetails", HelpText = "Show details of a stationinformation")]
public sealed class Details
{
    [Option("si", Required = true, HelpText = "Global Id of Sensor Information")]
    public string GlobalId { get; set; }
}

