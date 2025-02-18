using System;

namespace PR.Domain
{
    public interface IObjectWithGuidID
    {
        Guid ID { get; set; }
    }
}