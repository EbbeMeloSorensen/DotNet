using CommandLine;

namespace DMI.Data.Studio.UI.Console.Verbs;

[Verb("intervals", HelpText = "Retrieve observations intervals.")]
public sealed class Intervals
{
    [Option('s', "station id", Required = true, HelpText = "station id (nanoq)")]
    public string StatID { get; set; }

    [Option('p', "parameter", Required = true, HelpText = "parameter")]
    public string Parameter { get; set; }

    [Option('f', "first year", Required = true, HelpText = "first year")]
    public string FirstYear { get; set; }

    [Option('l', "last year", Required = true, HelpText = "last year")]
    public string LastYear { get; set; }

    [Option('t', "tolerance", Required = true, HelpText = "Maximum tolerable difference between two observations in hours")]
    public string Tolerance { get; set; }
}