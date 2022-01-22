using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public class GraphAdjacencyMatrix : IGraph
    {
        protected bool _directed;
        protected double[,] _adjacencyMatrix;
        protected int _vertexCount;

        public int VertexCount
        {
            get { return _vertexCount; }
        }

        public GraphAdjacencyMatrix(
            bool directed,
            int numberOfVertices)
        {
            _directed = directed;
            _adjacencyMatrix = new double[numberOfVertices, numberOfVertices];
            _vertexCount = numberOfVertices;
        }

        public void AddEdge(
            int vertexId1,
            int vertexId2,
            double cost)
        {
            _adjacencyMatrix[vertexId1, vertexId2] = cost;

            if (!_directed)
            {
                _adjacencyMatrix[vertexId2, vertexId1] = cost;
            }
        }

        public double GetCost(
            int vertexId1, 
            int vertexId2)
        {
            return _adjacencyMatrix[vertexId1, vertexId2];
        }

        public IEnumerable<int> NeighborIds(
            int vertexId)
        {
            for (var i = 0; i < VertexCount; i++)
            {
                if (_adjacencyMatrix[vertexId, i] > 0)
                {
                    yield return i;
                }
            }
        }
    }

    public class GraphAdjacencyMatrix<TV, TE> : GraphAdjacencyMatrix, IGraph<TV, TE> 
        where TV : IVertex
        where TE : IEdge
    {
        private static readonly ConstructorInfo _edgeConstructorInfo;
        private TE[] _edges;
        private Dictionary<int, List<TE>> _edgeMap;

        static GraphAdjacencyMatrix()
        {
            _edgeConstructorInfo = typeof(TE).GetConstructor(new[] { typeof(int), typeof(int) });

            if (_edgeConstructorInfo == null)
            {
                throw new InvalidOperationException("Edge class doesn't have a constructor that takes two integer arguments");
            }
        }

        public GraphAdjacencyMatrix(
            IEnumerable<TV> vertices,
            bool directed) : base(directed, vertices.Count())
        {
            _directed = directed;
            Vertices = vertices.ToArray();

            var vertexCount = vertices.Count();

            for (var i = 0; i < vertexCount; i++)
            {
                Vertices[i].Id = i;
            }

            _adjacencyMatrix = new double[vertexCount, vertexCount];
        }

        public TV[] Vertices { get; }

        public TE[] Edges
        {
            get
            {
                EnsureEdgesAreEstablished();

                return _edges;
            }
        }

        // Det her er ikke særligt pænt... Du skal kunne opdatere en vertex i grafen uden at fucke vertex id'er op
        // Husk de tildeles i constructoren
        public void UpdateVertex(int vertexId, TV vertex)
        {
            vertex.Id = vertexId;
            Vertices[vertexId] = vertex;
        }

        public IEnumerable<TE> GetAdjacentEdges(int vertexId)
        {
            EnsureEdgesAreEstablished();

            return _edgeMap[vertexId];
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