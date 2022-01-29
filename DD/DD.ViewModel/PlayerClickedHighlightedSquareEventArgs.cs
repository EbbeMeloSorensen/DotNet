using System;

namespace DD.ViewModel
{
    public class PlayerClickedSquareEventArgs : EventArgs
    {
        public readonly int SquareIndex;

        public PlayerClickedSquareEventArgs(int squareIndex)
        {
            SquareIndex = squareIndex;
        }
    }
}
