using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Craft.DataStructures.Graph
{
    // This is a template class for representing graphs where we want to associate some information
    // with vertices and/or edges such as a location of a vertex.
    // This class is suitable for graphs with many vertices.

    public class GraphAdjacencyList<TV, TE> : IGraph<TV, TE> 
        where TV : IVertex
        where TE : IEdge
    {
        private static readonly ConstructorInfo _edgeConstructorInfo;
        private List<Tuple<int, TE>>[] _adjacencyList;

        public string GetLabel(
            int vertexId)
        {
            return Vertices[vertexId].ToString();
        }

        public bool IsDirected { get; }

        public int VertexCount => Vertices.Count;

        public List<TV> Vertices { get; }

        public List<TE> Edges { get; }

        static GraphAdjacencyList()
        {
            _edgeConstructorInfo = typeof(TE).GetConstructor(new[] { typeof(int), typeof(int) });

            if (_edgeConstructorInfo == null)
            {
                throw new InvalidOperationException("Edge class doesn't have a constructor that takes two integer arguments");
            }
        }

        public GraphAdjacencyList(
            IEnumerable<TV> vertices,
            bool directed)
        {
            IsDirected = directed;
            Vertices = vertices.ToList();

            // Traverse vertices and assign each of them a unique Id
            for (var i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].Id = i;
            }

            Edges = new List<TE>();
            _adjacencyList = new List<Tuple<int, TE>>[Vertices.Count];
        }

        public void AddEdge(
            int vertexId1,
            int vertexId2)
        {
            var edge = (TE) _edgeConstructorInfo.Invoke(new object[] { vertexId1, vertexId2 });
            Edges.Add(edge);

            if (_adjacencyList[vertexId1] == null)
            {
                _adjacencyList[vertexId1] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[vertexId1].Add(new Tuple<int, TE>(vertexId2, edge));

            if (IsDirected)
            {
                return;
            }

            if (_adjacencyList[vertexId2] == null)
            {
                _adjacencyList[vertexId2] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[vertexId2].Add(new Tuple<int, TE>(vertexId1, edge));
        }

        public void AddEdge(
            TE edge)
        {
            Edges.Add(edge);

            if (_adjacencyList[edge.VertexId1] == null)
            {
                _adjacencyList[edge.VertexId1] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[edge.VertexId1].Add(new Tuple<int, TE>(edge.VertexId2, edge));
        }

        // Det her er ikke særligt pænt... Du skal kunne opdatere en vertex i grafen uden at fucke vertex id'er op
        // Husk de tildeles i constructoren
        // Hvad bruger vi overhovedet den her til? -> Den kaldes af MovePoint i en gui test.. så den bør nok fjernes og erstattes med noget andet
        public void UpdateVertex(int vertexId, TV vertex)
        {
            vertex.Id = vertexId;
            Vertices[vertexId] = vertex;
        }

        // Returns the edges that go to or from a given vertex
        public IEnumerable<TE> GetAdjacentEdges(int vertexId)
        {
            if (_adjacencyList[vertexId] == null)
            {
                return new List<TE>();
            }

            return _adjacencyList[vertexId].Select(_ => _.Item2);
        }
        
        public double GetCost(int vertexId1, int vertexId2)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> NeighborIds(int vertexId)
        {
            if (_adjacencyList[vertexId] == null)
            {
                return new List<int>();
            }

            return _adjacencyList[vertexId].Select(_ => _.Item1);
        }
    }
}