﻿using Craft.DataStructures.IO.graphml;
using Craft.DataStructures.IO.graphml.x;
using Craft.DataStructures.IO.graphml.y;
using Craft.DataStructures.IO.graphml.yjs;

namespace Craft.DataStructures.IO
{
    public static class NodeExtensionMethods
    {
        public static void AddZOrder(
            this node node,
            int value = 1)
        {
            node.nodeElements.Add(new data
            {
                key = "d0",
                value = value.ToString()
            });
        }

        public static void AddLabel(
            this node node,
            string label)
        {
            node.nodeElements.Add(new data
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
            });
        }

        public static void AddGeometry(
            this node node,
            RectD rectD)
        {
            node.nodeElements.Add(new data
            {
                key = "d5",
                RectD = rectD
            });
        }

        public static void AddStyle(
            this node node,
            string stroke,
            string fill)
        {
            node.nodeElements.Add(new data
            {
                key = "d7",
                ShapeNodeStyle = new ShapeNodeStyle
                {
                    stroke = stroke,
                    fill = fill
                }
            });
        }

        public static void AddPort(
            this node node)
        {
            node.nodeElements.Add(new port
            {
                name = "p0"
            });
        }
    }
}