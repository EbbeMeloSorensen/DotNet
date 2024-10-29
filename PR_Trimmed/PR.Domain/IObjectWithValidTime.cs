using System;

namespace PR.Domain
{
    public interface IObjectWithValidTime
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
    }
}