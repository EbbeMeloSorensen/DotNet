using Craft.Math;

namespace Simulator.Domain
{
    // A prop is a view only part of the scene, i.e. it doesn't affect the simulation but is
    // only concerned with stuff that should be visible in the scene
    public class Prop
    {
        public int Id { get; }
        public double Width { get; }
        public double Height { get; }
        public Vector2D Position { get; }
        public double Orientation { get; }

        public Prop(
            int id,
            double width,
            double height,
            Vector2D position,
            double orientation)
        {
            Id = id;
            Width = width;
            Height = height;
            Position = position;
            Orientation = orientation;
        }
    }
}
