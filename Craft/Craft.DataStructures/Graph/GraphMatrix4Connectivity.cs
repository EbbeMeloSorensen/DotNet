using System;
using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public class GraphMatrix4Connectivity : IGraph
    {
        private int _rows;
        private int _cols;

        public int VertexCount { get; }

        public string GetNodeLabel(
            int vertexId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDirected => false;

        public GraphMatrix4Connectivity(
            int rows, 
            int columns)
        {
            _rows = rows;
            _cols = columns;
            VertexCount = rows * columns;
        }

        public double GetCost(
            int vertexId1, 
            int vertexId2)
        {
            // I guess the cost of going from a vertex to its neighbor will always be one practically
            return 1;
        }

        public IEnumerable<int> NeighborIds(
            int vertexId)
        {
            var rowIndex = vertexId / _cols;
            var colIndex = vertexId % _cols;

            if (colIndex > 0)
            {
                yield return vertexId - 1;
            }

            if (colIndex < _cols - 1)
            {
                yield return vertexId + 1;
            }

            if (rowIndex > 0)
            {
                yield return vertexId - _cols;
            }

            if (rowIndex < _rows - 1)
            {
                yield return vertexId + _cols;
            }
        }

        public IEnumerable<IEdge> OutgoingEdges(
            int vertexId)
        {
            var rowIndex = vertexId / _cols;
            var colIndex = vertexId % _cols;

            if (colIndex > 0)
            {
                yield return new EdgeWithCost(vertexId, vertexId - 1, 1);
            }

            if (colIndex < _cols - 1)
            {
                yield return new EdgeWithCost(vertexId, vertexId + 1, 1);
            }

            if (rowIndex > 0)
            {
                yield return new EdgeWithCost(vertexId, vertexId - _cols, 1);
            }

            if (rowIndex < _rows - 1)
            {
                yield return new EdgeWithCost(vertexId, vertexId + _cols, 1);
            }
        }
    }
}