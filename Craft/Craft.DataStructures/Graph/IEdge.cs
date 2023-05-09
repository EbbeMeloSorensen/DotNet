namespace Craft.DataStructures.Graph
{
    public interface IEdge
    {
        int Id { get; set; }

        int VertexId1 { get; }
        int VertexId2 { get; }
    }
}