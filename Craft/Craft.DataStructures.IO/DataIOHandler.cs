using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Craft.DataStructures.Graph;
using Craft.DataStructures.IO.graphml;
using Craft.DataStructures.IO.graphml.x;
using Craft.DataStructures.IO.graphml.y;
using Craft.DataStructures.IO.graphml.yjs;

namespace Craft.DataStructures.IO
{
    public static class DataIOHandler
    {
        private static XmlSerializer _xmlSerializer;
        private static XmlSerializerNamespaces _xmlSerializerNamespaces;

        public static void SerializeGraphML(
            IGraph graph,
            string fileName)
        {
            var graphml = ConvertGraphToGraphML(graph);

            using var writer = new StreamWriter(fileName);
            XmlSerializer.Serialize(writer, graphml, XmlSerializerNamespaces);
            writer.Close();
        }

        private static XmlSerializer XmlSerializer
        {
            get
            {
                if (_xmlSerializer != null)
                    return _xmlSerializer;

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

                var attrs4 = new XmlAttributes();
                attrs4.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });

                var attrOverrides = new XmlAttributeOverrides();
                attrOverrides.Add(typeof(graphml.graphml), "graphmlElements", attrs1);
                attrOverrides.Add(typeof(graph), "graphElements", attrs2);
                attrOverrides.Add(typeof(node), "nodeElements", attrs3);
                attrOverrides.Add(typeof(edge), "edgeElements", attrs4);

                _xmlSerializer = new XmlSerializer(typeof(graphml.graphml), attrOverrides);

                return _xmlSerializer;
            }
        }

        private static XmlSerializerNamespaces XmlSerializerNamespaces
        {
            get
            {
                if (_xmlSerializerNamespaces != null)
                    return _xmlSerializerNamespaces;

                _xmlSerializerNamespaces = new XmlSerializerNamespaces();
                _xmlSerializerNamespaces.Add("demostyle2", "http://www.yworks.com/yFilesHTML/demos/FlatDemoStyle/2.0");
                _xmlSerializerNamespaces.Add("demostyle", "http://www.yworks.com/yFilesHTML/demos/FlatDemoStyle/1.0");
                _xmlSerializerNamespaces.Add("icon-style", "http://www.yworks.com/yed-live/icon-style/1.0");
                _xmlSerializerNamespaces.Add("bpmn", "http://www.yworks.com/xml/yfiles-bpmn/2.0");
                _xmlSerializerNamespaces.Add("demotablestyle", "http://www.yworks.com/yFilesHTML/demos/FlatDemoTableStyle/1.0");
                _xmlSerializerNamespaces.Add("uml", "http://www.yworks.com/yFilesHTML/demos/UMLDemoStyle/1.0");
                _xmlSerializerNamespaces.Add("GraphvizNodeStyle", "http://www.yworks.com/yFilesHTML/graphviz-node-style/1.0");
                _xmlSerializerNamespaces.Add("VuejsNodeStyle", "http://www.yworks.com/demos/yfiles-vuejs-node-style/1.0");
                _xmlSerializerNamespaces.Add("explorer-style", "http://www.yworks.com/data-explorer/1.0");
                _xmlSerializerNamespaces.Add("y", "http://www.yworks.com/xml/yfiles-common/3.0");
                _xmlSerializerNamespaces.Add("x", "http://www.yworks.com/xml/yfiles-common/markup/3.0");
                _xmlSerializerNamespaces.Add("yjs", "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml");
                _xmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                return _xmlSerializerNamespaces;
            }
        }

        private static graphml.graphml ConvertGraphToGraphML(
            IGraph graph)
        {
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
                edgedefault = graph.IsDirected ? edgedefault.directed : edgedefault.undirected,
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
                g.graphElements.Add(generateNode($"n{i}", -40, -163.5, null, graph.GetNodeLabel(i)));

                edges.AddRange(graph.OutgoingEdges(i)
                    .Select(edge => generateEdge($"e{edgeId++}", $"n{i}", $"n{edge.VertexId2}", "x")));
            }

            foreach (var edge in edges)
            {
                g.graphElements.Add(edge);
            }

            graphml.graphmlElements.Add(g);


            return graphml;
        }

        private static node generateNode(
            string nodeId,
            double X,
            double Y,
            int? zOrder = null,
            string? label = null)
        {
            var node = new node(nodeId);

            if (zOrder.HasValue)
            {
                node.AddZOrder(zOrder.Value);
            }

            if (label != null)
            {
                node.AddLabel(label);
            }

            node.AddGeometry(new RectD{ X = X, Y = Y, Width = 30, Height = 30});
            node.AddStyle("#FF663800", "#FFFF8C00");
            node.AddPort();

            return node;
        }

        private static edge generateEdge(
            string edgeId,
            string source,
            string target,
            string? label = null)
        {
            var edge = new edge
            {
                id = edgeId,
                source = source,
                target = target,
                sourceport = "p0",
                targetport = "p0",
                edgeElements = new ArrayList()
            };

            if (label != null)
            {
                edge.edgeElements.Add(
                new data
                {
                    key = "d11",
                    List = new List
                    {
                        Label = new Label
                        {
                            LabelText = label,
                            LayoutParameter = new LayoutParameter
                            {
                                SmartEdgeLabelModelParameter = new SmartEdgeLabelModelParameter
                                {
                                    Distance = 5,
                                    SmartEdgeLabelModelParameterModel = new SmartEdgeLabelModelParameterModel
                                    {
                                        SmartEdgeLabelModel = new SmartEdgeLabelModel()
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
                });
            }

            edge.edgeElements.Add(
            new data
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
            });

            return edge;
        }
    }
}
