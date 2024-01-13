namespace WIGOS.Domain.Entities.Context
{
    public enum ContextCategoryCode
    {
        Assessment,
        Correlation,
        OperationalInformationGroup,
        Overlay,
        Prediction
    }

    public class Context
    {
        public Guid Id { get; set; }
        public ContextCategoryCode ContextCategoryCode { get; set; }
        public string Name { get; set; }

        public Context()
        {
            Name = "";
        }
    }
}