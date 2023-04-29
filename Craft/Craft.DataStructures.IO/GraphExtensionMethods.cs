using System;
using System.Collections;
using Craft.DataStructures.Graph;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Craft.DataStructures.IO.graphml;
using System.Xml.Serialization;
using Craft.DataStructures.IO.graphml.x;
using Craft.DataStructures.IO.graphml.y;
using Craft.DataStructures.IO.graphml.yjs;

namespace Craft.DataStructures.IO
{
    public enum Format
    {
        Dot,
        GraphML
    }

    public static class GraphExtensionMethods
    {
        private static Dictionary<int, char> _letterMap = new Dictionary<int, char>
        {
            { 0, 'a' },
            { 1, 'b' },
            { 2, 'c' },
            { 3, 'd' },
            { 4, 'e' },
            { 5, 'f' },
            { 6, 'g' },
            { 7, 'h' },
            { 8, 'i' },
            { 9, 'j' },
            { 10, 'k' },
            { 11, 'l' },
            { 12, 'm' },
            { 13, 'n' },
            { 14, 'o' },
            { 15, 'p' },
            { 16, 'q' },
            { 17, 'r' },
            { 18, 's' },
            { 19, 't' },
            { 20, 'u' },
            { 21, 'v' },
            { 22, 'w' },
            { 23, 'x' },
            { 24, 'y' },
            { 25, 'z' },
        };

        public static void WriteToFile(
            this IGraph graph,
            string outputFile,
            Format format)
        {
            switch (format)
            {
                case Format.Dot:
                    using (var streamWriter = new StreamWriter(outputFile))
                    {
                        if (graph.IsDirected)
                        {
                            streamWriter.WriteLine("digraph {");

                            for (var vertexId1 = 0; vertexId1 < graph.VertexCount; vertexId1++)
                            {
                                var neighborIds = graph.NeighborIds(vertexId1).ToArray();

                                foreach (var vertexId2 in neighborIds)
                                {
                                    streamWriter.WriteLine($"   {_letterMap[vertexId1]} -> {_letterMap[vertexId2]};");
                                }
                            }
                        }
                        else
                        {
                            streamWriter.WriteLine("graph {");

                            for (var vertexId1 = 0; vertexId1 < graph.VertexCount; vertexId1++)
                            {
                                var neighborIds = graph.NeighborIds(vertexId1).ToArray();

                                foreach (var vertexId2 in neighborIds)
                                {
                                    if (vertexId1 < vertexId2)
                                    {
                                        streamWriter.WriteLine($"   {_letterMap[vertexId1]} -- {_letterMap[vertexId2]};");
                                    }
                                }
                            }
                        }

                        streamWriter.WriteLine("}");
                    }

                    break;
                case Format.GraphML:

                    var graphml = new graphml.graphml
                    {
                        graphmlElements = new ArrayList
                        {
                            new key
                            {
                                Id = "d0",
                                Domain = "node",
                                GraphMLAttributeType = "int",
                                GraphMLAttributeName = "zOrder",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-z-order/1.0/zOrder"
                            },
                            new key
                            {
                                Id = "d1",
                                Domain = "node",
                                GraphMLAttributeType = "boolean",
                                GraphMLAttributeName = "Expanded",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/Expanded",
                                @default = new @default()
                                {
                                    value = "true"
                                }
                            },
                            new key
                            {
                                Id = "d2",
                                Domain = "node",
                                GraphMLAttributeType = "string",
                                GraphMLAttributeName = "url"
                            },
                            new key
                            {
                                Id = "d3",
                                Domain = "node",
                                GraphMLAttributeType = "string",
                                GraphMLAttributeName = "description"
                            },
                            new key
                            {
                                Id = "d4",
                                Domain = "node",
                                GraphMLAttributeName = "NodeLabels",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/NodeLabels"
                            },
                            new key
                            {
                                Id = "d5",
                                Domain = "node",
                                GraphMLAttributeName = "NodeGeometry",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/NodeGeometry"
                            },
                            new key
                            {
                                Id = "d6",
                                Domain = "node",
                                GraphMLAttributeName = "UserTags",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/UserTags"
                            },
                            new key
                            {
                                Id = "d7",
                                Domain = "node",
                                GraphMLAttributeName = "NodeStyle",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/NodeStyle"
                            },
                            new key
                            {
                                Id = "d8",
                                Domain = "node",
                                GraphMLAttributeName = "NodeViewState",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/1.1/NodeViewState"
                            },
                            new key
                            {
                                Id = "d9",
                                Domain = "edge",
                                GraphMLAttributeType = "string",
                                GraphMLAttributeName = "url"
                            },
                            new key
                            {
                                Id = "d10",
                                Domain = "edge",
                                GraphMLAttributeType = "string",
                                GraphMLAttributeName = "description"
                            },
                            new key
                            {
                                Id = "d11",
                                Domain = "edge",
                                GraphMLAttributeName = "EdgeLabels",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/EdgeLabels"
                            },
                            new key
                            {
                                Id = "d12",
                                Domain = "edge",
                                GraphMLAttributeName = "EdgeGeometry",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/EdgeGeometry"
                            },
                            new key
                            {
                                Id = "d13",
                                Domain = "edge",
                                GraphMLAttributeName = "EdgeStyle",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/EdgeStyle"
                            },
                            new key
                            {
                                Id = "d14",
                                Domain = "edge",
                                GraphMLAttributeName = "EdgeViewState",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/1.1/EdgeViewState"
                            },
                            new key
                            {
                                Id = "d15",
                                Domain = "port",
                                GraphMLAttributeName = "PortLabels",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/PortLabels"
                            },
                            new key
                            {
                                Id = "d16",
                                Domain = "port",
                                GraphMLAttributeName = "PortLocationParameter",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/PortLocationParameter",
                                @default = new @default
                                {
                                    Static = new Static
                                    {
                                        Member = "y:FreeNodePortLocationModel.NodeCenterAnchored"
                                    }
                                }
                            },
                            new key
                            {
                                Id = "d17",
                                Domain = "port",
                                GraphMLAttributeName = "PortStyle",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/PortStyle",
                                @default = new @default
                                {
                                    Static = new Static
                                    {
                                        Member = "y:VoidPortStyle.Instance"
                                    }
                                }
                            },
                            new key
                            {
                                Id = "d18",
                                Domain = "port",
                                GraphMLAttributeName = "PortViewState",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/1.1/PortViewState"
                            },
                            new key
                            {
                                Id = "d19",
                                GraphMLAttributeName = "SharedData",
                                AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/SharedData"
                            },
                            new data
                            {
                                key = "d19",
                                SharedData = new SharedData
                                {
                                    InteriorLabelModel = new InteriorLabelModel
                                    {
                                        Key = 1
                                    }
                                }
                            }
                        }
                    };

                    var g = new graph
                    {
                        id = "G",
                        edgedefault = edgedefault.directed,
                        graphElements = new ArrayList
                        {
                            new data
                            {
                                key = "d6",
                                Json = new Json
                                {
                                    value =
                                        "{\"version\":\"2.0.0\",\"__mg\":\"yed-live\",\"theme\":{\"name\":\"light\",\"version\":\"1.0.0\"}}"
                                }
                            },
                        }
                    };

                    var edges = new List<edge>();
                    var edgeId = 0;

                    for (var i = 0; i < graph.VertexCount; i++)
                    {
                        g.graphElements.Add(generateNode($"n{i}", $"n{i}", -40, -163.5));

                        edges.AddRange(graph.NeighborIds(i).Select(j => generateEdge($"e{edgeId++})", $"n{i}", $"n{j}")));
                    }

                    foreach (var edge in edges)
                    {
                        g.graphElements.Add(edge);
                    }

                    graphml.graphmlElements.Add(g);

                    SerializeGraphML(graphml, outputFile);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static void SerializeGraphML(
            graphml.graphml graphml,
            string fileName)
        {
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
            var writer = new StreamWriter(fileName);

            var ns = new XmlSerializerNamespaces();
            ns.Add("demostyle2", "http://www.yworks.com/yFilesHTML/demos/FlatDemoStyle/2.0");
            ns.Add("demostyle", "http://www.yworks.com/yFilesHTML/demos/FlatDemoStyle/1.0");
            ns.Add("icon-style", "http://www.yworks.com/yed-live/icon-style/1.0");
            ns.Add("bpmn", "http://www.yworks.com/xml/yfiles-bpmn/2.0");
            ns.Add("demotablestyle", "http://www.yworks.com/yFilesHTML/demos/FlatDemoTableStyle/1.0");
            ns.Add("uml", "http://www.yworks.com/yFilesHTML/demos/UMLDemoStyle/1.0");
            ns.Add("GraphvizNodeStyle", "http://www.yworks.com/yFilesHTML/graphviz-node-style/1.0");
            ns.Add("VuejsNodeStyle", "http://www.yworks.com/demos/yfiles-vuejs-node-style/1.0");
            ns.Add("explorer-style", "http://www.yworks.com/data-explorer/1.0");
            ns.Add("y", "http://www.yworks.com/xml/yfiles-common/3.0");
            ns.Add("x", "http://www.yworks.com/xml/yfiles-common/markup/3.0");
            ns.Add("yjs", "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            serializer.Serialize(writer, graphml, ns);
            writer.Close();
        }

        private static node generateNode(
            string nodeId,
            string label,
            double X,
            double Y)
        {
            return new node
            {
                id = nodeId,
                nodeElements = new ArrayList
                {
                    new data
                    {
                        key = "d0",
                        value = "1"
                    },
                    new data
                    {
                        key = "d4",
                        List = new List
                        {
                            Label = new Label
                            {
                                LabelText = label,
                                LayoutParameter = new LayoutParameter
                                {
                                    CompositeLabelModelParameter = new CompositeLabelModelParameter
                                    {
                                        CompositeLabelModelParameterParameter =
                                            new CompositeLabelModelParameterParameter
                                            {
                                                InteriorLabelModelParameter = new InteriorLabelModelParameter
                                                {
                                                    Position = "Center",
                                                    Model = "{y:GraphMLReference 1}"
                                                }
                                            },
                                        CompositeLabelModelParameterModel = new CompositeLabelModelParameterModel
                                        {
                                            CompositeLabelModel = new CompositeLabelModel
                                            {
                                                CompositeLabelModelLabelModels = new CompositeLabelModelLabelModels
                                                {
                                                    ExteriorLabelModel = new ExteriorLabelModel
                                                    {
                                                        Insets = "5"
                                                    },
                                                    GraphMLReference = new GraphMLReference
                                                    {
                                                        ResourceKey = "1"
                                                    },
                                                    FreeNodeLabelModel = new FreeNodeLabelModel()
                                                }
                                            }
                                        }
                                    }
                                },
                                Style = new Style
                                {
                                    DefaultLabelStyle = new DefaultLabelStyle
                                    {
                                        verticalTextAlignment = "CENTER",
                                        horizontalTextAlignment = "CENTER",
                                        textFill = "BLACK",
                                        DefaultLabelStyleFont = new DefaultLabelStyleFont
                                        {
                                            Font = new Font
                                            {
                                                fontSize = 12,
                                                fontFamily = "'Arial'"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new data
                    {
                        key = "d5",
                        RectD = new RectD
                        {
                            X = X,
                            Y = Y,
                            Width = 30,
                            Height = 30
                        }   
                    },
                    new data
                    {
                        key = "d7",
                        ShapeNodeStyle = new ShapeNodeStyle
                        {
                            stroke = "#FF663800",
                            fill = "#FFFF8C00"
                        }
                    },
                    new port
                    {
                        name = "p0"
                    }
                }
            };
        }

        private static edge generateEdge(
            string edgeId,
            string source,
            string target)
        {
            return new edge
            {
                id = edgeId,
                source = source,
                target = target,
                sourceport = "p0",
                targetport = "p0",
                data = new data
                {
                    key = "d13",
                    PolylineEdgeStyle = new PolylineEdgeStyle
                    {
                        stroke = "#FF663800",
                        PolylineEdgeStyleTargetArrow = new PolylineEdgeStyleTargetArrow
                        {
                            Arrow = new Arrow
                            {
                                type = "TRIANGLE",
                                scale = 0.75,
                                stroke = "#FF663800",
                                fill = "#FF663800",
                                cropLength = 1
                            }
                        }
                    }
                }
            };
        }
    }
}
