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
        public void WriteASimpleDirectedGraphToGraphMLFile()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(true, 4);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(2, 3, 1);
            graph.AddEdge(3, 0, 1);

            var outputFile = @"C:\Temp\SimpleDirectedGraph.xml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);

            // Assert
            // Det skal gerne ramme det her: http://graphml.graphdrawing.org/primer/graphml-primer.html
            // men indtil videre laver den det hele som elementer og ikke attributter
        }
    }
}