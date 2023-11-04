using Simulator.Application;
using Simulator.Domain;

namespace Game.Rocket.ViewModel
{
    public class Level : ApplicationState
    {
        public Scene Scene { get; set; }

        public Level(
            string name) : base(name)
        {
        }
    }
}
