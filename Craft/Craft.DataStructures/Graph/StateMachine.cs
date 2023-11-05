using System.Collections.Generic;
using System.Linq;

namespace Craft.DataStructures.Graph
{
    // A state machine is a graph that has a node which is currently active.
    public class StateMachine : GraphAdjacencyList<State, LabelledEdge>
    {
        private int _currentStateIndex;

        public State CurrentState { get; private set; }

        public StateMachine(
            State initialState) : base(true)
        {
            AddState(initialState);
            CurrentState = initialState;
        }

        public StateMachine(
            IEnumerable<State> states) : base(states, true)
        {
            CurrentState = states.First();
        }

        public void AddState(
            State state)
        {
            AddVertex(state);
        }

        public void AddTransition(
            State from,
            State to,
            string name = null)
        {
            AddEdge(new LabelledEdge(from.Id, to.Id, name));
        }

        public IEnumerable<string> ExitsFromCurrentState()
        {
            return OutgoingEdges(_currentStateIndex)
                .Select(_ => (_ as LabelledEdge).Label);
        }

        public void SwitchState(
            string transitionName = null)
        {
            _currentStateIndex = transitionName == null 
                ? OutgoingEdges(_currentStateIndex).Single().VertexId2 
                : OutgoingEdges(_currentStateIndex).Single(_ => (_ as LabelledEdge).Label == transitionName).VertexId2;

            CurrentState = Vertices[_currentStateIndex];
        }
    }
}
