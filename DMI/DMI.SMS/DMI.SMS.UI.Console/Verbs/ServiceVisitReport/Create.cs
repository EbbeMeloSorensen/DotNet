using CommandLine;

namespace DMI.SMS.UI.Console.Verbs.ServiceVisitReport;

[Verb("createServiceVisitReport", HelpText = "Create Service Visit Report")]
public sealed class Create
{
    [Option("si", Required = true, HelpText = "Global Id of Sensor Information")]
    public string ParentGuid { get; set; }
}
