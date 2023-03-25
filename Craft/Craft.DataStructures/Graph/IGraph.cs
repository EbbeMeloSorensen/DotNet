using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public interface IGraph
    {
        int VertexCount { get; }

        double GetCost(
            int vertexId1,
            int vertexId2);

        IEnumerable<int> NeighborIds(
            int vertexId);

        bool IsDirected { get; }
    }

    // The 'out' keywords indicate that the type parameters are declared as covariant,
    // which is apparently recommended
    public interface IGraph<out TV, out TE> : IGraph 
        where TV : IVertex 
        where TE : IEdge 
    {
        TV[] Vertices { get; }

        TE[] Edges { get; }
    }
}
