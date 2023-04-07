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

    public class GraphAdjacencyList<TV, TE> : /*GraphAdjacencyMatrix,*/ IGraph<TV, TE> 
        where TV : IVertex
        where TE : IEdge
    {
        private static readonly ConstructorInfo _edgeConstructorInfo;
        private List<Tuple<int, TE>>[] _adjacencyList;

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

            if (_adjacencyList[vertexId2] == null)
            {
                _adjacencyList[vertexId2] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[vertexId2].Add(new Tuple<int, TE>(vertexId1, edge));
        }

        public void AddEdge(
            int vertexId1,
            int vertexId2,
            TE edge)
        {
            Edges.Add(edge);
            _adjacencyList[vertexId1].Add(new Tuple<int, TE>(vertexId2, edge));
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
            //EnsureEdgesAreEstablished();

            //return _edgeMap[vertexId];

            //throw new NotImplementedException();

            if (_adjacencyList[vertexId] == null)
            {
                return new List<TE>();
            }

            return _adjacencyList[vertexId].Select(_ => _.Item2);
        }

        //private void EnsureEdgesAreEstablished()
        //{
        //    if (_edges != null) return;

        //    if (_directed)
        //    {
        //        ComputeEdgesDirected();
        //    }
        //    else
        //    {
        //        ComputeEdgesUndirected();
        //    }
        //}

        //private void ComputeEdgesDirected()
        //{
        //    _edgeMap = new Dictionary<int, List<TE>>();
        //    var temp = new List<TE>();
        //    var vertexCount = _adjacencyMatrix.GetLength(0);

        //    for (var i = 0; i < vertexCount; i++)
        //    {
        //        for (var j = 0; j < vertexCount; j++)
        //        {
        //            if (!(_adjacencyMatrix[i, j] > 0)) continue;

        //            var edge = (TE) _edgeConstructorInfo.Invoke(new object[] {i, j});

        //            temp.Add(edge);
        //            AddEdgeToMap(i, edge);
        //            AddEdgeToMap(j, edge);
        //        }
        //    }

        //    _edges = temp.ToArray();
        //}

        //private void ComputeEdgesUndirected()
        //{
        //    _edgeMap = new Dictionary<int, List<TE>>();
        //    var temp = new List<TE>();
        //    var vertexCount = _adjacencyMatrix.GetLength(0);

        //    for (var i = 0; i < vertexCount; i++)
        //    {
        //        for (var j = i + 1; j < vertexCount; j++)
        //        {
        //            if (!(_adjacencyMatrix[i, j] > 0)) continue;

        //            var edge = (TE) _edgeConstructorInfo.Invoke(new object[] {i, j});

        //            temp.Add(edge);
        //            AddEdgeToMap(i, edge);
        //            AddEdgeToMap(j, edge);
        //        }
        //    }

        //    _edges = temp.ToArray();
        //}

        private void AddEdgeToMap(
            int vertexId, 
            TE edge)
        {
            //if (!_edgeMap.ContainsKey(vertexId))
            //{
            //    _edgeMap[vertexId] = new List<TE>();
            //}

            //_edgeMap[vertexId].Add(edge);
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