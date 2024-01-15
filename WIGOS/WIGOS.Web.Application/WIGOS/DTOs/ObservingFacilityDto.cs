namespace WIGOS.Web.Application.WIGOS.DTOs
{
    public class ObservingFacilityDto : AbstractEnvironmentalMonitoringFacilityDto
    {
        public string? Name { get; set; }
        public DateTime? DateEstablished { get; set; }
        public DateTime? DateClosed { get; set; }

        public ObservingFacilityDto()
        {
            Category = "Observing Facility";
        }
    }
}