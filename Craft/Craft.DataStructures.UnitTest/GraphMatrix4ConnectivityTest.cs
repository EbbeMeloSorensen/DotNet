using System.Linq;
using Xunit;
using FluentAssertions;

namespace Craft.DataStructures.Graph.UnitTest
{
    public class GraphMatrix4ConnectivityTest
    {
        [Fact]
        public void CheckNeigborship()
        {
            var graph = new GraphMatrix4Connectivity(4, 3);

            graph.VertexCount.Should().Be(12);
            graph.NeighborIds(0).SequenceEqual(new[] { 1, 3 }).Should().BeTrue();
            graph.NeighborIds(1).SequenceEqual(new[] { 0, 2, 4 }).Should().BeTrue();
            graph.NeighborIds(2).SequenceEqual(new[] { 1, 5 }).Should().BeTrue();
            graph.NeighborIds(3).SequenceEqual(new[] { 4, 0, 6 }).Should().BeTrue();
            graph.NeighborIds(4).SequenceEqual(new[] { 3, 5, 1, 7 }).Should().BeTrue();
            graph.NeighborIds(5).SequenceEqual(new[] { 4, 2, 8 }).Should().BeTrue();
            graph.NeighborIds(6).SequenceEqual(new[] { 7, 3, 9 }).Should().BeTrue();
            graph.NeighborIds(7).SequenceEqual(new[] { 6, 8, 4, 10 }).Should().BeTrue();
            graph.NeighborIds(8).SequenceEqual(new[] { 7, 5, 11 }).Should().BeTrue();
            graph.NeighborIds(9).SequenceEqual(new[] { 10, 6 }).Should().BeTrue();
            graph.NeighborIds(10).SequenceEqual(new[] { 9, 11, 7 }).Should().BeTrue();
            graph.NeighborIds(11).SequenceEqual(new[] { 10, 8 }).Should().BeTrue();
        }
    }
}