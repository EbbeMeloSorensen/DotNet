using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Algorithms.GuiTest.Tab1
{
    public enum Mode
    {
        Source,
        Forbidden
    }

    public class Tab1ViewModel
    {
        private Mode _currentMode = Mode.Source;
        private HashSet<int> _sourceIndexes;
        private HashSet<int> _forbiddenIndexes;
        //private List<PixelViewModel> _pixelViewModels;

    }
}
