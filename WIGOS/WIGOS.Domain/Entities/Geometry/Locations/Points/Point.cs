namespace WIGOS.Domain.Entities.Geometry.Locations.Points
{
    public abstract class Point : Location
    {
        // Todo: pr�v at lave dette som en extension method i stedet
        public abstract List<double> AsListOfDouble();

        protected Point(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}