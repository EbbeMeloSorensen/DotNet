using Craft.Math;

namespace Simulator.Domain.BodyStates
{
    public class BodyStateCyclic : BodyState
    {
        protected BodyStateCyclic(
            Body body) : base(body)
        {
        }

        public BodyStateCyclic(
            Body body, 
            Vector2D position) : base(body, position)
        {
        }
    }
}
