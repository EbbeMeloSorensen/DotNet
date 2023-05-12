using System;
using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public class GraphMatrix8Connectivity : IGraph
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

        public GraphMatrix8Connectivity(
            int rows, 
            int columns)
        {
            _rows = rows;
            _cols = columns;
            VertexCount = rows * columns;
        }

        public IEnumerable<IEdge> OutgoingEdges(
            int vertexId)
        {
            var rowIndex = vertexId / _cols;
            var colIndex = vertexId % _cols;

            if (colIndex > 0)
            {
                //yield return vertexId - 1;
                yield return new EdgeWithCost(vertexId, vertexId - 1, 1);

                if (rowIndex > 0)
                {
                    //yield return vertexId - _cols - 1;
                    yield return new EdgeWithCost(vertexId, vertexId - _cols - 1, Math.Sqrt(2.0));
                }

                if (rowIndex < _rows - 1)
                {
                    //yield return vertexId + _cols - 1;
                    yield return new EdgeWithCost(vertexId, vertexId + _cols - 1, Math.Sqrt(2.0));
                }
            }

            if (rowIndex > 0)
            {
                //yield return vertexId - _cols;
                yield return new EdgeWithCost(vertexId, vertexId - _cols, 1);

                if (colIndex < _cols - 1)
                {
                    //yield return vertexId - _cols + 1;
                    yield return new EdgeWithCost(vertexId, vertexId - _cols + 1, Math.Sqrt(2.0));
                }
            }

            if (colIndex < _cols - 1)
            {
                //yield return vertexId + 1;
                yield return new EdgeWithCost(vertexId, vertexId + 1, 1);

                if (rowIndex < _rows - 1)
                {
                    //yield return vertexId + _cols + 1;
                    yield return new EdgeWithCost(vertexId, vertexId + _cols + 1, Math.Sqrt(2));
                }
            }

            if (rowIndex < _rows - 1)
            {
                //yield return vertexId + _cols;
                yield return new EdgeWithCost(vertexId, vertexId + _cols, 1);
            }
        }
    }
}