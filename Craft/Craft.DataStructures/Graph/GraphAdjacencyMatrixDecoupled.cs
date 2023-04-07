using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Craft.DataStructures.Graph
{
    // This is a template class for representing graphs where we want to associate some information
    // with vertices and/or edges such as a location of a vertex.
    // Note that it derives from GraphAdjacencyMatrix, so it utilizes a matrix for storing cost,
    // making it ill suited for graphs with many vertices.
    // This class facilitates inspection of individual vertices and edges, and it facilitates retrieval
    // of the edges that go in and out of a given vertex

    // Pt kan man kun ændre i matricen ved at kalde basisklassens AddEdge metode

    public class GraphAdjacencyMatrixDecoupled<TV, TE> : GraphAdjacencyMatrix, IGraph<TV, TE> 
        where TV : IVertex
        where TE : IEdge
    {
        private static readonly ConstructorInfo _edgeConstructorInfo;
        private TE[] _edges;
        private Dictionary<int, List<TE>> _edgeMap;

        public TV[] Vertices { get; }

        public TE[] Edges
        {
            get
            {
                EnsureEdgesAreEstablished();

                return _edges;
            }
        }

        static GraphAdjacencyMatrixDecoupled()
        {
            _edgeConstructorInfo = typeof(TE).GetConstructor(new[] { typeof(int), typeof(int) });

            if (_edgeConstructorInfo == null)
            {
                throw new InvalidOperationException("Edge class doesn't have a constructor that takes two integer arguments");
            }
        }

        public GraphAdjacencyMatrixDecoupled(
            IEnumerable<TV> vertices,
            bool directed) : base(directed, vertices.Count())
        {
            Vertices = vertices.ToArray();

            // Traverse vertices and assign each of them a unique Id
            for (var i = 0; i < _vertexCount; i++)
            {
                Vertices[i].Id = i;
            }
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
            EnsureEdgesAreEstablished();

            return _edgeMap[vertexId];
        }

        private void EnsureEdgesAreEstablished()
        {
            if (_edges != null) return;

            if (_directed)
            {
                ComputeEdgesDirected();
            }
            else
            {
                ComputeEdgesUndirected();
            }
        }

        private void ComputeEdgesDirected()
        {
            _edgeMap = new Dictionary<int, List<TE>>();
            var temp = new List<TE>();
            var vertexCount = _adjacencyMatrix.GetLength(0);

            for (var i = 0; i < vertexCount; i++)
            {
                for (var j = 0; j < vertexCount; j++)
                {
                    if (!(_adjacencyMatrix[i, j] > 0)) continue;

                    var edge = (TE) _edgeConstructorInfo.Invoke(new object[] {i, j});

                    temp.Add(edge);
                    AddEdgeToMap(i, edge);
                    AddEdgeToMap(j, edge);
                }
            }

            _edges = temp.ToArray();
        }

        private void ComputeEdgesUndirected()
        {
            _edgeMap = new Dictionary<int, List<TE>>();
            var temp = new List<TE>();
            var vertexCount = _adjacencyMatrix.GetLength(0);

            for (var i = 0; i < vertexCount; i++)
            {
                for (var j = i + 1; j < vertexCount; j++)
                {
                    if (!(_adjacencyMatrix[i, j] > 0)) continue;

                    var edge = (TE) _edgeConstructorInfo.Invoke(new object[] {i, j});

                    temp.Add(edge);
                    AddEdgeToMap(i, edge);
                    AddEdgeToMap(j, edge);
                }
            }

            _edges = temp.ToArray();
        }

        private void AddEdgeToMap(
            int vertexId, 
            TE edge)
        {
            if (!_edgeMap.ContainsKey(vertexId))
            {
                _edgeMap[vertexId] = new List<TE>();
            }

            _edgeMap[vertexId].Add(edge);
        }
    }
}