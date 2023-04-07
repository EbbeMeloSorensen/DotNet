using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Craft.DataStructures.Graph.UnitTest
{
    public class GraphAdjancencyMatrixTest
    {
        [Fact]
        public void BuildAGraphAdjancencyMatrix_UsingConstructorThatTakesNumberOfVertices()
        {
            // Act
            var graph = new GraphAdjacencyMatrix(false, 3);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Assert
            graph.VertexCount.Should().Be(3);
            graph.NeighborIds(0).SequenceEqual(new[] { 1, 2 }).Should().BeTrue();
        }

        [Fact]
        public void BuildAGraphAdjancencyMatrix_UsingConstructorThatTakesVertexSequence()
        {
            // Arrange
            var vertices = new List<Point2DVertex>
            {
                new Point2DVertex(0, 0),
                new Point2DVertex(4, 0),
                new Point2DVertex(4, 3),
            };

            // Act
            var graph = new GraphAdjacencyMatrixDecoupled<Point2DVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Assert
            graph.VertexCount.Should().Be(3);
            graph.NeighborIds(0).SequenceEqual(new[] { 1, 2 }).Should().BeTrue();
        }

        [Fact]
        public void TraverseAllVerticesOfAGraph()
        {
            // Arrange
            var vertices = new List<Point2DVertex>
            {
                new Point2DVertex(0, 0),
                new Point2DVertex(4, 0),
                new Point2DVertex(4, 3),
            };

            var graph = new GraphAdjacencyMatrixDecoupled<Point2DVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Act/Assert
            graph.Vertices.Count().Should().Be(3);
        }

        [Fact]
        public void ChangeAVertexInAGraph()
        {
            // Arrange
            var vertices = new List<Point2DVertex>
            {
                new Point2DVertex(0, 0),
                new Point2DVertex(4, 0),
                new Point2DVertex(4, 3),
            };

            var graph = new GraphAdjacencyMatrixDecoupled<Point2DVertex, EmptyEdge>(vertices, false);

            // Act
            graph.Vertices[0] = new Point2DVertex(1, 1);

            // Assert
            graph.Vertices[0].X.Should().Be(1);
        }

        [Fact]
        public void TraverseAllEdgesOfADirectedGraph()
        {
            // Arrange
            var vertices = Enumerable.Repeat(new EmptyVertex(), 3);
            var graph = new GraphAdjacencyMatrixDecoupled<EmptyVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Act
            var edges = graph.Edges.ToList();

            // Assert
            edges.Count.Should().Be(3);
            edges[0].VertexId1.Should().Be(0);
            edges[0].VertexId2.Should().Be(1);
            edges[1].VertexId1.Should().Be(0);
            edges[1].VertexId2.Should().Be(2);
            edges[2].VertexId1.Should().Be(1);
            edges[2].VertexId2.Should().Be(2);
        }

        [Fact]
        public void TraverseAllEdgesOfAnUnDirectedGraph()
        {
            // Arrange
            var vertices = Enumerable.Repeat(new EmptyVertex(), 3);
            var graph = new GraphAdjacencyMatrixDecoupled<EmptyVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Act
            var edges = graph.Edges.ToList();

            // Assert
            edges.Count.Should().Be(3);
        }

        [Fact]
        public void AskAGraphForAnEdgeTwiceAndVerifyItReturnsTheSameObject()
        {
            // Arrange
            var vertices = Enumerable.Repeat(new EmptyVertex(), 3);
            var graph = new GraphAdjacencyMatrixDecoupled<EmptyVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Act
            var edges1 = graph.Edges.ToList();
            var edges2 = graph.Edges.ToList();

            // Assert
            edges1.First().Equals(edges2.First()).Should().BeTrue();
        }

        [Fact]
        public void GetAdjacentEdges_ReturnsCorrectResult()
        {
            // Arrange
            var vertices = Enumerable.Repeat(new EmptyVertex(), 3);
            var graph = new GraphAdjacencyMatrixDecoupled<EmptyVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(0, 2, 1);
            graph.AddEdge(1, 2, 1);

            // Act
            var edges = graph.GetAdjacentEdges(2);

            // Assert
            edges.Count().Should().Be(2);
            edges.First().GetOppositeVertexId(2).Should().Be(0);
            edges.Skip(1).First().GetOppositeVertexId(2).Should().Be(1);
        }
    }
}