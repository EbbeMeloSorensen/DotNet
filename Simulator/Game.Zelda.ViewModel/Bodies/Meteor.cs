using Simulator.Domain;

namespace Game.Zelda.ViewModel.Bodies
{
    public class Meteor : CircularBody
    {
        public Meteor(
            int id,
            double radius,
            double mass,
            bool affectedByGravity) : base(id, radius, mass, affectedByGravity)
        {
        }
    }
}