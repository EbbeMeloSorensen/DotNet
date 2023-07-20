using CommandLine;

namespace DMI.ObsDB.SimpleDataMigrator.Verbs
{
    [Verb("migrate", HelpText = "Migrate.")]
    public sealed class Migrate
    {
        [Option('y', "first_year", Required = false, HelpText = "First Year")]
        public int FirstYear { get; set; }

        [Option('q', "last_year", Required = false, HelpText = "Last Year")]
        public int LastYear { get; set; }

        [Option('l', "station_limit", Required = false, HelpText = "Station Limit")]
        public int? StationLimit { get; set; }
    }
}
