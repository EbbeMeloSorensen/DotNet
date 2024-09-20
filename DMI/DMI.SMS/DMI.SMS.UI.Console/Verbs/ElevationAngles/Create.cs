using CommandLine;

namespace DMI.SMS.UI.Console.Verbs.ElevationAngles;

[Verb("createElevationAngles", HelpText = "Create Elevation Angles")]
public sealed class Create
{
    [Option('n', "north", Required = true, HelpText = "North")]
    public int Angle_N { get; set; }
}