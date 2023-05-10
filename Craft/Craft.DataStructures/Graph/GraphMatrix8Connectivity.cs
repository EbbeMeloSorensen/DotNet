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

        public double GetCost(
            int vertexId1, 
            int vertexId2)
        {
            // The cost will either be 1 or sqrt(2) depending on if you go from white to black or white to white
            var rowIndex1 = vertexId1 / _cols;
            var colIndex1 = vertexId1 % _cols;

            var rowIndex2 = vertexId2 / _cols;
            var colIndex2 = vertexId2 % _cols;

            var vertexId1IsBlack = rowIndex1 % 2 == 0
                ? colIndex1 % 2 == 0
                : colIndex1 % 2 == 1;

            var vertexId2IsBlack = rowIndex2 % 2 == 0
                ? colIndex2 % 2 == 0
                : colIndex2 % 2 == 1;

            return vertexId1IsBlack == vertexId2IsBlack ? System.Math.Sqrt(2.0) : 1;
        }

        public IEnumerable<int> NeighborIds(
            int vertexId)
        {
            var rowIndex = vertexId / _cols;
            var colIndex = vertexId % _cols;

            if (colIndex > 0)
            {
                yield return vertexId - 1;

                if (rowIndex > 0)
                {
                    yield return vertexId - _cols - 1;
                }

                if (rowIndex < _rows - 1)
                {
                    yield return vertexId + _cols - 1;
                }
            }

            if (rowIndex > 0)
            {
                yield return vertexId - _cols;

                if (colIndex < _cols - 1)
                {
                    yield return vertexId - _cols + 1;
                }
            }

            if (colIndex < _cols - 1)
            {
                yield return vertexId + 1;

                if (rowIndex < _rows - 1)
                {
                    yield return vertexId + _cols + 1;
                }
            }

            if (rowIndex < _rows - 1)
            {
                yield return vertexId + _cols;
            }
        }
    }
}