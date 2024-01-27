using System;
using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public class GraphHexMesh : IGraph
    {
        private int _rows;
        private int _cols;

        public int VertexCount { get; }

        public bool IsDirected => false;

        public GraphHexMesh(
            int rows,
            int columns)
        {
            _rows = rows;
            _cols = columns;
            VertexCount = rows * columns;
        }

        public IVertex GetVertex(
            int vertexId)
        {
            return new EmptyVertex();
        }

        public IEnumerable<IEdge> OutgoingEdges(
            int vertexId)
        {
            var rowIndex = vertexId / _cols;
            var colIndex = vertexId % _cols;

            throw new NotImplementedException();
        }
    }
}
