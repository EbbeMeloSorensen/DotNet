using System;

namespace C2IEDM.ViewModel;

public class DatabaseWriteOperationOccuredEventArgs : EventArgs
{
    public DateTime DateTime { get; }

    public DatabaseWriteOperationOccuredEventArgs(
        DateTime dateTime)
    {
        DateTime = dateTime;
    }
}