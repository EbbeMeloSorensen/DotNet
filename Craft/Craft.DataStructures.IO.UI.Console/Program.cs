using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;
using Craft.DataStructures.IO.graphml;

void ParseDotFile(
    string dotFile,
    out GraphAdjacencyList<LabelledVertex, EmptyEdge> graph)
{
    var lines = File.ReadAllLines(dotFile);

    var graphType = lines.First().Split(' ').First();
    Console.WriteLine(graphType);

    graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(true);
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
}

void CollectGraphDataFromDotFile(
    string dotFile,
    HashSet<string> vertexLabels,
    List<Tuple<string, string>> edgeData)
{
    var lines = File.ReadAllLines(dotFile);

    foreach (var line in lines.Skip(1))
    {
        if (line.Contains('}'))
        {
            break;
        }

        if (line.Contains(" -> "))
        {
            var temp = line.TrimStart().TrimEnd(';').Split(' ');
            var sourceVertexLabel = temp.First();
            var targetVertexLabel = temp.Last();
            vertexLabels.Add(sourceVertexLabel);
            vertexLabels.Add(targetVertexLabel);
            edgeData.Add(new Tuple<string, string>(sourceVertexLabel, targetVertexLabel));
        }
        else
        {
            var vertexLabel = line.TrimStart().TrimEnd(';').Split(' ').Single();
            vertexLabels.Add(vertexLabel);
        }
    }
}

var vertexLabels = new HashSet<string>();
var edgeData = new List<Tuple<string, string>>();

//CollectGraphDataFromDotFile(@"C:\Temp\graph1.dot", vertexLabels, edgeData);
//CollectGraphDataFromDotFile(@"C:\Temp\graph2.dot", vertexLabels, edgeData);

var directory = new DirectoryInfo("C:\\Users\\B053687\\Git\\DMI_Gitlab\\enterprise-architecture\\catalogs_generated\\aci_dot_files");
foreach (var file in directory.GetFiles())
{
    CollectGraphDataFromDotFile(file.FullName, vertexLabels, edgeData);
}

var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(true);
var vertexIdMap = new Dictionary<string, int>();

foreach (var vertexLabel in vertexLabels)
{
    var vertex = new LabelledVertex(vertexLabel);
    graph.AddVertex(vertex);
    vertexIdMap[vertexLabel] = vertex.Id; 
}

foreach (var item in edgeData)
{
    var vertexId1 = vertexIdMap[item.Item1];
    var vertexId2 = vertexIdMap[item.Item2];

    graph.AddEdge(vertexId1, vertexId2);
}

graph.WriteToFile(@"C:\Temp\Take1.graphml", Format.GraphML);
Console.WriteLine("Done");

if (false)
{
    ParseDotFile(@"C:\Temp\SimpleDirectedGraph.dot", out var graph2);
    graph.WriteToFile(@"C:\Temp\Inferno.graphml", Format.GraphML);
    Console.WriteLine("Done");
}
