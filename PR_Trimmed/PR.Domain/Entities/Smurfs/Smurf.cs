using System;

namespace PR.Domain.Entities.Smurfs
{
    public class Smurf
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
