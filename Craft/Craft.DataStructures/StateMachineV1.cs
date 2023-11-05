using System.Linq;
using System.Collections.Generic;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures
{
    public class StateMachineV1
    {
        private GraphAdjacencyList<LabelledVertex, LabelledEdge> _graph;
        private Dictionary<StateV1, int> _vertexIdMap;
        private Dictionary<int, StateV1> _stateMap;
        private int _currentIndex;

        public StateV1 StateV1 => _stateMap[_currentIndex];

        public List<string> ExitsFromCurrentState => _graph.GetAdjacentEdges(_currentIndex)
            .Select(_ => _.Label)
            .ToList();

        public StateMachineV1(
            IEnumerable<StateV1> states)
        {
            _graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(
                states.Select(_ => new LabelledVertex(_.Name)), true);

            var temp = states
                .Select((state, index) => new { State = state, Index = index })
                .ToArray();

            _vertexIdMap = temp
                .ToDictionary(_ => _.State, _ => _.Index);

            _stateMap = temp
                .ToDictionary(_ => _.Index, _ => _.State);

            _currentIndex = 0;
        }

        public void AddState(
            StateV1 stateV1)
        {
            //_graph.

        }

        public void AddTransition(
            StateV1 from,
            StateV1 to,
            string label = null)
        {
            _graph.AddEdge(new LabelledEdge(_vertexIdMap[from], _vertexIdMap[to], label));
        }

        public void SwitchState()
        {
            _currentIndex = _graph.GetAdjacentEdges(_currentIndex).Single().VertexId2;
        }

        public void SwitchState(
            string transitionLabel)
        {
            _currentIndex = _graph.GetAdjacentEdges(_currentIndex)
                .Single(_ => _.Label == transitionLabel).VertexId2;
        }
    }
}
