namespace WIGOS.Web.Application.WIGOS.DTOs
{
    public abstract class AbstractEnvironmentalMonitoringFacilityDto
    {
        public string Category { get; set; }
        public Guid Id { get; set; }

        public AbstractEnvironmentalMonitoringFacilityDto()
        {
        }
    }
}