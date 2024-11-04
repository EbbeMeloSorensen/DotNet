using System;

namespace PR.Domain.Wigos
{
    public abstract class VersionedObject
    {
        public Guid ObjectId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }

        public VersionedObject(
            Guid objectId,
            DateTime created)
        {
            ObjectId = objectId;
            Created = created;
            Superseded = DateTime.MaxValue;
        }
    }

    public abstract class AbstractEnvironmentalMonitoringFacility : VersionedObject
    {
        public Guid Id { get; set; }

        protected AbstractEnvironmentalMonitoringFacility(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }

    public class ObservingFacility : AbstractEnvironmentalMonitoringFacility
    {
        public string? Name { get; set; }
        public DateTime DateEstablished { get; set; }
        public DateTime DateClosed { get; set; }

        public ObservingFacility(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }

        public override string ToString()
        {
            return $"Observing Facility: {Name}";
        }
    }

    public abstract class GeospatialLocation : VersionedObject
    {
        public Guid Id { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public Guid AbstractEnvironmentalMonitoringFacilityId { get; set; }
        public Guid AbstractEnvironmentalMonitoringFacilityObjectId { get; set; }
        public virtual AbstractEnvironmentalMonitoringFacility AbstractEnvironmentalMonitoringFacility { get; set; }

        public GeospatialLocation(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }

    public class Point : GeospatialLocation
    {
        public string CoordinateSystem { get; set; }
        public double Coordinate1 { get; set; }
        public double Coordinate2 { get; set; }

        public Point(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }

        public override string ToString()
        {
            var result = $"({Coordinate1}, {Coordinate2}), From: {From.ToShortDateString()}";

            if (To < DateTime.MaxValue)
            {
                result += $" To: {To.ToShortDateString()}";
            }

            return result;
        }
    }
}
