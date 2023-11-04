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
        public void BuildAGraphAdjancencyList_Directed()
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
            graph.AddEdge(new LabelledEdge(0, 1, "wife"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            // Assert
            graph.Vertices.Count.Should().Be(4);
            graph.GetAdjacentEdges(0).Count().Should().Be(1);
            graph.GetAdjacentEdges(1).Count().Should().Be(0);
            graph.GetAdjacentEdges(2).Count().Should().Be(2);
            graph.GetAdjacentEdges(3).Count().Should().Be(2);
            graph.GetAdjacentEdges(0).First().Label.Should().Be("wife");
        }
    }
}
