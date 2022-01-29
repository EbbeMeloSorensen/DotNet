using Simulator.Domain;

namespace Simulator.Laboratory.ViewModel
{
    public class Projectile : CircularBody
    {
        public Projectile(
            int id,
            double radius,
            double mass,
            bool affectedByGravity) : base(id, radius, mass, affectedByGravity)
        {
        }
    }
}