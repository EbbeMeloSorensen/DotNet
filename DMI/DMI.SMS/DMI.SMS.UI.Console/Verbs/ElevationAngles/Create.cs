using CommandLine;

namespace DMI.SMS.UI.Console.Verbs.ElevationAngles;

[Verb("createElevationAngles", HelpText = "Create Elevation Angles")]
public sealed class Create
{
    [Option(shortName: 'n', Required = true, HelpText = "North")]
    public int Angle_N { get; set; }

    [Option("ne", Required = true, HelpText = "North East")]
    public int Angle_NE { get; set; }

}