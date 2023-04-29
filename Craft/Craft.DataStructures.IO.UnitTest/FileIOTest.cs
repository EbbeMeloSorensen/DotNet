using System.Linq;
using Xunit;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures.IO.UnitTest
{
    public class FileIOTest
    {
        [Fact]
        public void WriteASimpleUndirectedGraphToDotFile()
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
            graph.WriteToFile(outputFile, Format.Dot);

            // Assert
            // (Install GraphViz and execute e.g. dot -Tsvg SimpleUndirectedGraph.dot > SimpleUndirectedGraph.svg)
        }

        [Fact]
        public void WriteASimpleDirectedGraphToDotFile()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(true, 4);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(2, 3, 1);
            graph.AddEdge(3, 0, 1);

            var outputFile = @"C:\Temp\SimpleDirectedGraph.dot";

            // Act
            graph.WriteToFile(outputFile, Format.Dot);
        }

        [Fact]
        public void WriteGraphAdjacencyMatrixToGraphMLFile()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(true, 4);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(2, 3, 1);
            graph.AddEdge(3, 0, 1);

            var outputFile = @"C:\Temp\GraphAdjacencyMatrix.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile()
        {
            // Arrange
            var vertices = Enumerable.Repeat(0, 5).Select(_ => new EmptyVertex());

            var graph = new GraphAdjacencyList<EmptyVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 0);

            var outputFile = @"C:\Temp\GraphAdjacencyList.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }
    }
}