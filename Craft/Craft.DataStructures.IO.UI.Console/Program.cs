using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;

var lines = File.ReadAllLines(@"C:\Temp\SimpleDirectedGraph.dot");

var graphType = lines.First().Split(' ').First();
Console.WriteLine(graphType);

var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(true);
var dictionary = new Dictionary<string, int>();

foreach (var line in lines.Skip(1))
{
    if (line.Contains('}'))
    {
        break;
    }

    Console.WriteLine(line);

    var temp = line.TrimStart().TrimEnd(';').Split(' ');

    var sourceNodeName = temp.First();
    var targetNodeName = temp.Last();

    if (!dictionary.ContainsKey(sourceNodeName))
    {
        var vertex = new LabelledVertex(sourceNodeName);
        graph.AddVertex(vertex);
        dictionary[sourceNodeName] = vertex.Id;
    }

    if (!dictionary.ContainsKey(targetNodeName))
    {
        var vertex = new LabelledVertex(targetNodeName);
        graph.AddVertex(vertex);
        dictionary[targetNodeName] = vertex.Id;
    }

    var idOfSourceVertex = dictionary[sourceNodeName];
    var idOfTargetVertex = dictionary[targetNodeName];

    graph.AddEdge(idOfSourceVertex, idOfTargetVertex);
}

graph.WriteToFile(@"C:\Temp\Inferno.graphml", Format.GraphML);

Console.WriteLine("Done");
