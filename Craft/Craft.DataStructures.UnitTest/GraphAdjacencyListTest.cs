using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures.UnitTest
{
    public class GraphAdjacencyListTest
    {
        [Fact]
        public void BuildAGraphAdjancencyMatrix_UsingConstructorThatTakesNumberOfVertices()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Ebbe"),
                new LabelledVertex("Ana"),
                new LabelledVertex("Anton"),
                new LabelledVertex("Cecilie")
            };

            var graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(vertices, true);
            graph.AddEdge(0, 1);
        }
    }
}
