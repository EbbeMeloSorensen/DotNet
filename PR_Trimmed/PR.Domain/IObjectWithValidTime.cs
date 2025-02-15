using System;

namespace PR.Domain
{
    public interface IObjectWithValidTime : IVersionedObject
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
    }
}