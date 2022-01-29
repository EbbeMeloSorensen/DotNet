using Simulator.Domain;

namespace Simulator.Laboratory.ViewModel
{
    public class Player1Rectangular : RectangularBody
    {
        public Player1Rectangular(
            int id,
            double width,
            double height,
            double mass,
            bool affectedByGravity) : base(id, width, height, mass, affectedByGravity)
        {
        }
    }
}