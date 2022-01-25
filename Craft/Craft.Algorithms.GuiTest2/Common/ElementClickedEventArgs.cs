using System;

namespace Craft.Algorithms.GuiTest2.Common
{
    public class ElementClickedEventArgs : EventArgs
    {
        public readonly int ElementId;

        public ElementClickedEventArgs(
            int elementId)
        {
            ElementId = elementId;
        }
    }
}