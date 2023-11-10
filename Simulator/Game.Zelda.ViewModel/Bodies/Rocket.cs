using Simulator.Domain;

namespace Game.Zelda.ViewModel.Bodies
{
    public class Rocket : CircularBody
    {
        public Rocket(
            int id,
            double radius,
            double mass,
            bool affectedByGravity) : base(id, radius, mass, affectedByGravity)
        {
        }
    }
}