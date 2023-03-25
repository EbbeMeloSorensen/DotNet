using Xunit;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures.IO.UnitTest
{
    public class FileIOTest
    {
        [Fact]
        public void DrawASimpleUndirectedGraph()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(false, 5);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(0, 2, 1);
            graph.AddEdge(3, 2, 1);
            graph.AddEdge(4, 2, 1);
            graph.AddEdge(4, 0, 1);

            var outputFile = @"C:\Temp\SimpleUndirectedGraph.dot";

            // Act
            graph.WriteToFile(outputFile);

            // Assert
            // (Install GraphViz and execute e.g. dot -Tsvg SimpleUndirectedGraph.dot > SimpleUndirectedGraph.svg)
        }

        [Fact]
        public void DrawASimpleDirectedGraph()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(true, 4);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(2, 3, 1);
            graph.AddEdge(3, 0, 1);

            var outputFile = @"C:\Temp\SimpleDirectedGraph.dot";

            // Act
            graph.WriteToFile(outputFile);
        }

    }
}