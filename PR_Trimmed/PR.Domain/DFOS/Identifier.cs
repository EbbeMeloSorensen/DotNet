namespace PR.Domain.DFOS
{
    public class Identifier
    {
        public IdentifierType IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string ValidPeriod { get; set; }
        public bool PrimaryWithinNamingScheme { get; set; }
    }
}
