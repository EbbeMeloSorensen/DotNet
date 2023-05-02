using System;
using System.Collections.Generic;
using System.Text;

namespace Craft.DataStructures.Graph
{
    public class LabelledVertex : IVertex
    {
        public int Id { get; set; }

        public string Label { get; }

        public LabelledVertex(
            string label)
        {
            Label = label;
        }

        public override string ToString()
        {
            return $"{Label}";
        }
    }
}
