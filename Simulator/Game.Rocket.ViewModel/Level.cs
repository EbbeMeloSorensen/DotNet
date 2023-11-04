using Simulator.Domain;

namespace Game.Rocket.ViewModel
{
    public class Level : Craft.DataStructures.State
    {
        public Scene Scene { get; set; }

        public Level(
            string name) : base(name)
        {
        }
    }
}
