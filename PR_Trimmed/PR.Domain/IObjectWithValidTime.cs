using System;

namespace PR.Domain
{
    public interface IObjectWithValidTime
    {
        Guid IntervalID { get; set; }
        DateTime Start { get; set; }
        DateTime End { get; set; }
    }
}