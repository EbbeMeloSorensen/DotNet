using System;

namespace WIGOS.ViewModel
{
    public class DatabaseWriteOperationOccuredEventArgs : EventArgs
    {
        public DateTime DateTime { get; }

        public DatabaseWriteOperationOccuredEventArgs(
            DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
}