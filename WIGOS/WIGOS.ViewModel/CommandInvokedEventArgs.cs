using System;

namespace WIGOS.ViewModel
{
    public class CommandInvokedEventArgs : EventArgs
    {
        public object Owner { get; }

        public CommandInvokedEventArgs(
            object owner)
        {
            Owner = owner;
        }
    }
}
