using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public class StateMachine : GraphAdjacencyList<LabelledVertex, LabelledEdge>
    {
        public StateMachine() : base(true)
        {
        }

        public StateMachine(
            IEnumerable<LabelledVertex> vertices) : base(vertices, true)
        {
        }
    }
}
