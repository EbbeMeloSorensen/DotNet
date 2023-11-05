using System.Linq;
using System.Collections.Generic;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures
{
    public class StateMachine
    {
        private GraphAdjacencyList<LabelledVertex, LabelledEdge> _graph;
        private Dictionary<State, int> _vertexIdMap;
        private Dictionary<int, State> _stateMap;
        private int _currentIndex;

        public State State => _stateMap[_currentIndex];

        public List<string> ExitsFromCurrentState => _graph.GetAdjacentEdges(_currentIndex)
            .Select(_ => _.Label)
            .ToList();

        public StateMachine(
            IEnumerable<State> states)
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
            State state)
        {
            //_graph.

        }

        public void AddTransition(
            State from,
            State to,
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
