namespace Craft.DataStructures.Graph
{
    public class Edge : IEdge
    {
        public int VertexId1 { get; }
        public int VertexId2 { get; }

        public Edge(
            int vertexId1, 
            int vertexId2)
        {
            VertexId1 = vertexId1;
            VertexId2 = vertexId2;
        }
    }
}