﻿using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;

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

// Windows
var useLinux = true;

var rootDirectory = useLinux
    //? "/home/ebs/Git/enterprise-architecture/catalogs_generated/aci_dot_files"
    ? "/home/ebs/Git/enterprise-architecture/catalogs_generated/dot_files_from_marc"
    : "C:\\Users\\B053687\\Git\\DMI_Gitlab\\enterprise-architecture\\catalogs_generated\\aci_dot_files";

var outputFile = useLinux
    ? "aci_topoplogy_marc.graphml"
    : @"C:\Temp\Take1.graphml";

var directory = new DirectoryInfo(rootDirectory);

Console.WriteLine($"Parsing yml files:");

foreach (var file in directory.GetFiles())
{
    Console.WriteLine($"  {file.Name}");
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

    if (vertexId1 == vertexId2)
    {
        Console.WriteLine($"    WARNING: {item.Item1} apparently connects to itself...");
    }

    graph.AddEdge(vertexId1, vertexId2);
}


Console.WriteLine($"Wrinting {outputFile}..");
graph.WriteToFile(outputFile, Format.GraphML);
Console.WriteLine("Done");

if (false)
{
    ParseDotFile(@"C:\Temp\SimpleDirectedGraph.dot", out var graph2);
    graph.WriteToFile(@"C:\Temp\Inferno.graphml", Format.GraphML);
    Console.WriteLine("Done");
}