using Craft.Math;

namespace Simulator.Domain.Props
{
    public class PropRectangle : Prop
    {
        public PropRectangle(
            int id,
            double width,
            double height,
            Vector2D position) : base(id)
        {
            Width = width;
            Height = height;
            Position = position;
        }

        public double Width { get; }
        public double Height { get; }
        public Vector2D Position { get; }
    }
}