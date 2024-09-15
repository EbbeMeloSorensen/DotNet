using CommandLine;

namespace DMI.SMS.UI.Console.Verbs.StationInformation;

[Verb("createStationInformation", HelpText = "Create Station Information")]
public sealed class Create
{
    [Option('i', "station id", Required = true, HelpText = "Station Id")]
    public string StationId { get; set; }

    [Option('n', "name", Required = true, HelpText = "Station Name")]
    public string StationName { get; set; }
}