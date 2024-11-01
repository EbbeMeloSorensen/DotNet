namespace Domain.Entities.Reporting
{
    public enum ReferenceSecurityClassificationCode
    {
        NatoUnclassified,
        NatoRestricted,
        NatoConfidential,
        NatoSecret,
        CosmicTopSecret
    }

    public class Reference
    {
        public Guid Id { get; set; }
        public ReferenceSecurityClassificationCode? ReferenceSecurityClassificationCode { get; set; }
        public string DescriptionText { get; set; }
        public string SourceText { get; set; }

        public Reference()
        {
            DescriptionText = "";
            SourceText = "";
        }
    }
}