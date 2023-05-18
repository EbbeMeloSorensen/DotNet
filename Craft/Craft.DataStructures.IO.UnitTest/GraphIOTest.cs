using System.Collections.Generic;
using System.Linq;
using Xunit;
using Craft.DataStructures.Graph;
using Craft.DataStructures.IO.graphml;
using System.Xml.Serialization;
using System.IO;

namespace Craft.DataStructures.IO.UnitTest
{
    public class GraphIOTest
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
        public void WriteGraphAdjacencyListToGraphMLFile_EmptyVertexAndEmptyEdgeDirected()
        {
            // Arrange
            var vertices = Enumerable.Repeat(0, 5).Select(_ => new EmptyVertex());

            var graph = new GraphAdjacencyList<EmptyVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 0);

            var outputFile = @"C:\Temp\GraphAdjacencyList_directed.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_EmptyVertexAndEmptyEdgeUndirected()
        {
            // Arrange
            var vertices = Enumerable.Repeat(0, 5).Select(_ => new EmptyVertex());

            var graph = new GraphAdjacencyList<EmptyVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 0);

            var outputFile = @"C:\Temp\GraphAdjacencyList_undirected.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_LabelledVertexAndEmptyEdge()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Denmark"),        //  0
                new LabelledVertex("Sweden"),         //  1
                new LabelledVertex("Norway"),         //  2
                new LabelledVertex("Germany"),        //  3
                new LabelledVertex("United Kingdom"), //  4
                new LabelledVertex("Ireland"),        //  5
                new LabelledVertex("Netherlands"),    //  6
                new LabelledVertex("Belgium"),        //  7
                new LabelledVertex("France"),         //  8
                new LabelledVertex("Luxembourg"),     //  9
                new LabelledVertex("Finland"),        // 10
                new LabelledVertex("Spain"),          // 11
                new LabelledVertex("Portugal"),       // 12
                new LabelledVertex("Italy"),          // 13
                new LabelledVertex("Switzerland"),    // 14
                new LabelledVertex("Austria"),        // 15
                new LabelledVertex("Czech Republic"), // 16
                new LabelledVertex("Poland"),         // 17
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1);
            graph.AddEdge(0, 3);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 10);
            graph.AddEdge(2, 10);
            graph.AddEdge(3, 6);
            graph.AddEdge(3, 7);
            graph.AddEdge(3, 8);
            graph.AddEdge(3, 9);
            graph.AddEdge(3, 14);
            graph.AddEdge(3, 15);
            graph.AddEdge(3, 16);
            graph.AddEdge(3, 17);
            graph.AddEdge(4, 5);
            graph.AddEdge(6, 7);
            graph.AddEdge(7, 8);
            graph.AddEdge(7, 9);
            graph.AddEdge(8, 9);
            graph.AddEdge(8, 11);
            graph.AddEdge(8, 13);
            graph.AddEdge(8, 14);
            graph.AddEdge(11, 12);
            graph.AddEdge(13, 14);
            graph.AddEdge(13, 15);
            graph.AddEdge(14, 15);
            graph.AddEdge(15, 16);
            graph.AddEdge(16, 17);

            var outputFile = @"C:\Temp\GraphAdjacencyList_labelledVertices.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_LabelledVertexAndLabelledEdge()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Ebbe"),   //  0
                new LabelledVertex("Ana"),    //  1
                new LabelledVertex("Anton"),  //  2
                new LabelledVertex("Cecilie") //  3
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(new LabelledEdge(0, 1, "spouse"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            var outputFile = @"C:\Temp\GraphAdjacencyList_labelledVertices_2.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        // Eksperimenteren med deserialisering af en graphml fil
        [Fact]
        public void ReadGraphAdjacencyListFromGraphMLFile()
        {
            // Arrange
            // Vi laver en serializer fuldstændigt som hvis vi skulle SKRIVE til en fil
            var attrs1 = new XmlAttributes();
            attrs1.XmlElements.Add(new XmlElementAttribute { ElementName = "key", Type = typeof(key) });
            attrs1.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });
            attrs1.XmlElements.Add(new XmlElementAttribute { ElementName = "graph", Type = typeof(graph) });

            var attrs2 = new XmlAttributes();
            attrs2.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });
            attrs2.XmlElements.Add(new XmlElementAttribute { ElementName = "node", Type = typeof(node) });
            attrs2.XmlElements.Add(new XmlElementAttribute { ElementName = "edge", Type = typeof(edge) });

            var attrs3 = new XmlAttributes();
            attrs3.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });
            attrs3.XmlElements.Add(new XmlElementAttribute { ElementName = "port", Type = typeof(port) });

            var attrOverrides = new XmlAttributeOverrides();
            attrOverrides.Add(typeof(graphml.graphml), "graphmlElements", attrs1);
            attrOverrides.Add(typeof(graph), "graphElements", attrs2);
            attrOverrides.Add(typeof(node), "nodeElements", attrs3);

            var serializer = new XmlSerializer(typeof(graphml.graphml), attrOverrides);

            var inputFileName = @"C:\Temp\GraphAdjacencyList_labelledVertices.graphml";

            var fs = new FileStream(inputFileName, FileMode.Open);
            var g = (graphml.graphml)serializer.Deserialize(fs);

            // Det kan man tilsyneladende fint - så kan vi lige så godt placere den Serializer et centralt sted
        }
    }
}