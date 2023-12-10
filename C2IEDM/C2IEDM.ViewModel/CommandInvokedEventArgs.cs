using System;

namespace C2IEDM.ViewModel
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
