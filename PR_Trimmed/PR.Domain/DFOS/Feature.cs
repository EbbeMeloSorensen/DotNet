using System;

namespace PR.Domain.DFOS
{
    public class Feature
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }
}
